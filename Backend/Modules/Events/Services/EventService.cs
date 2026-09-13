using System.Security.Claims;
using CampusServicesPortal.Modules.Events.DTOs.Events;
using CampusServicesPortal.Modules.Events.Entities;
using CampusServicesPortal.Modules.Events.Interfaces.Repositories;
using CampusServicesPortal.Modules.Events.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace CampusServicesPortal.Modules.Events.Services;

public sealed class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IEventSeatRepository _eventSeatRepository;
    private readonly IEventRegistrationRepository _eventRegistrationRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EventService(
        IEventRepository eventRepository,
        IVenueRepository venueRepository,
        IEventSeatRepository eventSeatRepository,
        IEventRegistrationRepository eventRegistrationRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _eventRepository = eventRepository;
        _venueRepository = venueRepository;
        _eventSeatRepository = eventSeatRepository;
        _eventRegistrationRepository = eventRegistrationRepository;
        _httpContextAccessor = httpContextAccessor;
    }


    // =====================================================
    // GET ALL EVENTS
    // =====================================================
    public async Task<IEnumerable<EventResponseDto>> GetAllAsync(
        EventFilterDto filter)
    {
        var events =
            await _eventRepository.GetAllAsync(filter);

        var result =
            new List<EventResponseDto>();

        foreach (var eventEntity in events)
        {
            var activeRegistrationCount =
                await _eventRegistrationRepository
                    .GetActiveRegistrationCountAsync(
                        eventEntity.EventId);

            result.Add(
                MapToResponseDto(
                    eventEntity,
                    activeRegistrationCount));
        }

        return result;
    }


    // =====================================================
    // GET EVENT BY ID
    // =====================================================
    public async Task<EventResponseDto?> GetByIdAsync(
        int eventId)
    {
        var eventEntity =
            await _eventRepository.GetByIdAsync(
                eventId);

        if (eventEntity is null)
        {
            return null;
        }

        var activeRegistrationCount =
            await _eventRegistrationRepository
                .GetActiveRegistrationCountAsync(
                    eventId);

        return MapToResponseDto(
            eventEntity,
            activeRegistrationCount);
    }


    // =====================================================
    // CREATE EVENT
    // =====================================================
    public async Task<EventResponseDto> CreateAsync(
        CreateEventDto dto)
    {
        ValidateBasicEventData(
            dto.StartAt,
            dto.EndAt,
            dto.Capacity,
            dto.Title);


        var venue =
            await _venueRepository.GetByIdAsync(
                dto.VenueId);

        if (venue is null)
        {
            throw new KeyNotFoundException(
                $"Venue with ID {dto.VenueId} was not found.");
        }

        if (!venue.IsActive)
        {
            throw new InvalidOperationException(
                "The selected venue is inactive.");
        }

        if (dto.Capacity > venue.Capacity)
        {
            throw new InvalidOperationException(
                "Event capacity cannot exceed the venue capacity.");
        }


        // =================================================
        // VENUE OVERLAP CHECK
        // =================================================
        var isVenueAvailable =
            await _venueRepository
                .IsVenueAvailableAsync(
                    dto.VenueId,
                    dto.StartAt,
                    dto.EndAt);

        if (!isVenueAvailable)
        {
            throw new InvalidOperationException(
                "The selected venue is not available for the given time.");
        }


        // =================================================
        // RESERVED SEATING EVENT CANNOT BE CREATED
        // DIRECTLY AS PUBLISHED.
        //
        // Event must first be created unpublished,
        // seats configured,
        // then published through Update.
        // =================================================
        if (dto.UsesReservedSeating &&
            dto.IsPublished)
        {
            throw new InvalidOperationException(
                "A reserved-seating event must be created as unpublished first. Configure all seats before publishing.");
        }


        var userId =
            GetCurrentUserId();


        var eventEntity =
            new Event
            {
                VenueId =
                    dto.VenueId,

                CreatedByUserId =
                    userId,

                Title =
                    dto.Title.Trim(),

                Description =
                    string.IsNullOrWhiteSpace(
                        dto.Description)
                        ? null
                        : dto.Description.Trim(),

                StartAt =
                    dto.StartAt,

                EndAt =
                    dto.EndAt,

                Capacity =
                    dto.Capacity,

                UsesReservedSeating =
                    dto.UsesReservedSeating,

                IsPublished =
                    dto.IsPublished,

                IsActive =
                    true
            };


        await _eventRepository.AddAsync(
            eventEntity);

        await _eventRepository.SaveChangesAsync();


        var createdEvent =
            await _eventRepository.GetByIdAsync(
                eventEntity.EventId);

        if (createdEvent is null)
        {
            throw new InvalidOperationException(
                "Event was created but could not be retrieved.");
        }


        return MapToResponseDto(
            createdEvent,
            0);
    }


    // =====================================================
    // UPDATE EVENT
    // =====================================================
    public async Task UpdateAsync(
        int eventId,
        UpdateEventDto dto)
    {
        ValidateBasicEventData(
            dto.StartAt,
            dto.EndAt,
            dto.Capacity,
            dto.Title);


        var eventEntity =
            await _eventRepository.GetByIdAsync(
                eventId);

        if (eventEntity is null)
        {
            throw new KeyNotFoundException(
                $"Event with ID {eventId} was not found.");
        }


        var venue =
            await _venueRepository.GetByIdAsync(
                dto.VenueId);

        if (venue is null)
        {
            throw new KeyNotFoundException(
                $"Venue with ID {dto.VenueId} was not found.");
        }

        if (!venue.IsActive)
        {
            throw new InvalidOperationException(
                "The selected venue is inactive.");
        }

        if (dto.Capacity > venue.Capacity)
        {
            throw new InvalidOperationException(
                "Event capacity cannot exceed the venue capacity.");
        }


        // =================================================
        // CORRECT VENUE OVERLAP CHECK
        //
        // Current eventId is excluded.
        // So the event does not conflict with itself.
        // =================================================
        var isVenueAvailable =
            await _venueRepository
                .IsVenueAvailableAsync(
                    dto.VenueId,
                    dto.StartAt,
                    dto.EndAt,
                    eventId);

        if (!isVenueAvailable)
        {
            throw new InvalidOperationException(
                "The selected venue is not available for the given time.");
        }


        // =================================================
        // ACTIVE REGISTRATION COUNT
        //
        // Confirmed
        // + unexpired Held
        // =================================================
        var activeRegistrationCount =
            await _eventRegistrationRepository
                .GetActiveRegistrationCountAsync(
                    eventId);


        // =================================================
        // CAPACITY CANNOT BE REDUCED BELOW
        // CURRENT ACTIVE REGISTRATIONS
        // =================================================
        if (dto.Capacity <
            activeRegistrationCount)
        {
            throw new InvalidOperationException(
                $"Event capacity cannot be reduced below the current active registration count of {activeRegistrationCount}.");
        }


        // =================================================
        // RESERVED SEATING PUBLISH VALIDATION
        //
        // Before publishing:
        // active configured seats MUST equal capacity.
        // =================================================
        if (dto.UsesReservedSeating &&
            dto.IsPublished)
        {
            var seats =
                await _eventSeatRepository
                    .GetByEventIdAsync(
                        eventId);

            var activeSeatCount =
                seats.Count(s =>
                    s.IsActive);

            if (activeSeatCount !=
                dto.Capacity)
            {
                throw new InvalidOperationException(
                    $"A reserved-seating event cannot be published until the number of active seats equals the event capacity. Capacity: {dto.Capacity}, Active seats: {activeSeatCount}.");
            }
        }


        // =================================================
        // UPDATE ENTITY
        // =================================================
        eventEntity.VenueId =
            dto.VenueId;

        eventEntity.Title =
            dto.Title.Trim();

        eventEntity.Description =
            string.IsNullOrWhiteSpace(
                dto.Description)
                ? null
                : dto.Description.Trim();

        eventEntity.StartAt =
            dto.StartAt;

        eventEntity.EndAt =
            dto.EndAt;

        eventEntity.Capacity =
            dto.Capacity;

        eventEntity.UsesReservedSeating =
            dto.UsesReservedSeating;

        eventEntity.IsPublished =
            dto.IsPublished;

        eventEntity.IsActive =
            dto.IsActive;


        _eventRepository.Update(
            eventEntity);

        await _eventRepository
            .SaveChangesAsync();
    }


    // =====================================================
    // DELETE / DEACTIVATE EVENT
    // =====================================================
    public async Task DeleteAsync(
        int eventId)
    {
        var eventEntity =
            await _eventRepository.GetByIdAsync(
                eventId);

        if (eventEntity is null)
        {
            throw new KeyNotFoundException(
                $"Event with ID {eventId} was not found.");
        }


        eventEntity.IsActive =
            false;

        eventEntity.IsPublished =
            false;


        _eventRepository.Update(
            eventEntity);

        await _eventRepository
            .SaveChangesAsync();
    }


    // =====================================================
    // COMMON VALIDATION
    // =====================================================
    private static void ValidateBasicEventData(
        DateTime startAt,
        DateTime endAt,
        int capacity,
        string title)
    {
        if (string.IsNullOrWhiteSpace(
                title))
        {
            throw new ArgumentException(
                "Event title is required.");
        }

        if (startAt >= endAt)
        {
            throw new ArgumentException(
                "Event start time must be before event end time.");
        }

        if (capacity <= 0)
        {
            throw new ArgumentException(
                "Event capacity must be greater than zero.");
        }
    }


    // =====================================================
    // GET CURRENT AUTHENTICATED USER ID
    // =====================================================
    private int GetCurrentUserId()
    {
        var userIdValue =
            _httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(
                    ClaimTypes.NameIdentifier);

        if (!int.TryParse(
                userIdValue,
                out var userId))
        {
            throw new UnauthorizedAccessException(
                "Authenticated user ID could not be determined.");
        }

        return userId;
    }


    // =====================================================
    // MAP ENTITY -> RESPONSE DTO
    //
    // IMPORTANT:
    // registeredCount is ACTIVE count only.
    // Cancelled / Expired records are not counted.
    // =====================================================
    private static EventResponseDto MapToResponseDto(
        Event eventEntity,
        int registeredCount)
    {
        return new EventResponseDto
        {
            EventId =
                eventEntity.EventId,

            VenueId =
                eventEntity.VenueId,

            VenueName =
                eventEntity.Venue?.Name
                ?? string.Empty,

            Title =
                eventEntity.Title,

            Description =
                eventEntity.Description,

            StartAt =
                eventEntity.StartAt,

            EndAt =
                eventEntity.EndAt,

            Capacity =
                eventEntity.Capacity,

            RegisteredCount =
                registeredCount,

            AvailableSeats =
                Math.Max(
                    eventEntity.Capacity -
                    registeredCount,
                    0),

            UsesReservedSeating =
                eventEntity.UsesReservedSeating,

            IsPublished =
                eventEntity.IsPublished,

            IsActive =
                eventEntity.IsActive,

            CreatedAt =
                eventEntity.CreatedAt,

            UpdatedAt =
                eventEntity.UpdatedAt
        };
    }
}