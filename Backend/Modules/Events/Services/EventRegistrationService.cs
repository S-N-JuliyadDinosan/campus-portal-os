using System.Data;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Events.DTOs.EventRegistrations;
using CampusServicesPortal.Modules.Events.Entities;
using CampusServicesPortal.Modules.Events.Enums;
using CampusServicesPortal.Modules.Events.Interfaces.Repositories;
using CampusServicesPortal.Modules.Events.Interfaces.Services;
using CampusServicesPortal.Modules.Notifications.DTOs;
using CampusServicesPortal.Modules.Notifications.Interfaces.Services;
using CampusServicesPortal.Modules.SystemSettings.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Events.Services;

public sealed class EventRegistrationService
    : IEventRegistrationService
{
    private readonly IEventRegistrationRepository _registrationRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IEventSeatRepository _eventSeatRepository;
    private readonly INotificationService _notificationService;
    private readonly ApplicationDbContext _context;

    public EventRegistrationService(
        IEventRegistrationRepository registrationRepository,
        IEventRepository eventRepository,
        IEventSeatRepository eventSeatRepository,
        INotificationService notificationService,
        ApplicationDbContext context)
    {
        _registrationRepository = registrationRepository;
        _eventRepository = eventRepository;
        _eventSeatRepository = eventSeatRepository;
        _notificationService = notificationService;
        _context = context;
    }


    // =====================================================
    // REGISTER FOR EVENT
    //
    // BOTH:
    // - General admission
    // - Reserved seating
    //
    // start as HELD.
    // =====================================================
    public async Task<EventRegistrationResponseDto> RegisterAsync(
        int studentId,
        CreateEventRegistrationDto dto)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var eventEntity =
            await _eventRepository.GetByIdAsync(
                dto.EventId);

        if (eventEntity is null)
        {
            throw new KeyNotFoundException(
                $"Event with ID {dto.EventId} was not found.");
        }

        if (!eventEntity.IsActive)
        {
            throw new InvalidOperationException(
                "This event is no longer active.");
        }

        if (!eventEntity.IsPublished)
        {
            throw new InvalidOperationException(
                "This event is not currently available for registration.");
        }

        var now =
            DateTime.UtcNow;

        if (eventEntity.StartAt <= now)
        {
            throw new InvalidOperationException(
                "Registration is closed because the event has already started.");
        }


        // =================================================
        // ONE STUDENT = ONE ACTIVE REGISTRATION PER EVENT
        //
        // Active means:
        // - Confirmed
        // - Held and not expired
        //
        // Cancelled / Expired can register again.
        // =================================================
        var existingRegistrations =
            await _registrationRepository
                .GetActiveRegistrationsByStudentAsync(
                    studentId,
                    dto.EventId);

        if (existingRegistrations.Any())
        {
            throw new InvalidOperationException(
                "You already have an active registration for this event.");
        }


        // =================================================
        // EVENT CAPACITY
        //
        // Held + Confirmed both consume capacity.
        // =================================================
        var activeRegistrationCount =
            await _registrationRepository
                .GetActiveRegistrationCountAsync(
                    dto.EventId);

        if (activeRegistrationCount >=
            eventEntity.Capacity)
        {
            throw new InvalidOperationException(
                "This event has reached its maximum capacity.");
        }


        int? selectedSeatId = null;


        // =================================================
        // RESERVED SEATING EVENT
        // =================================================
        if (eventEntity.UsesReservedSeating)
        {
            if (!dto.EventSeatId.HasValue)
            {
                throw new InvalidOperationException(
                    "A seat must be selected for this event.");
            }


            var seatId =
                dto.EventSeatId.Value;

            var seat =
                await _eventSeatRepository
                    .GetByIdAsync(seatId);

            if (seat is null)
            {
                throw new KeyNotFoundException(
                    $"Event seat with ID {seatId} was not found.");
            }

            if (seat.EventId !=
                dto.EventId)
            {
                throw new InvalidOperationException(
                    "The selected seat does not belong to this event.");
            }

            if (!seat.IsActive)
            {
                throw new InvalidOperationException(
                    "The selected seat is inactive.");
            }


            // ---------------------------------------------
            // Seat must not already be:
            // - Confirmed
            // - Held and not expired
            // ---------------------------------------------
            var activeSeatRegistration =
                await _registrationRepository
                    .GetActiveSeatRegistrationAsync(
                        seatId);

            if (activeSeatRegistration is not null)
            {
                throw new InvalidOperationException(
                    "The selected seat is currently unavailable.");
            }

            selectedSeatId =
                seatId;
        }

        // =================================================
        // GENERAL ADMISSION EVENT
        //
        // Individual seat must NOT be supplied.
        // =================================================
        else
        {
            if (dto.EventSeatId.HasValue)
            {
                throw new InvalidOperationException(
                    "A seat cannot be selected for a general admission event.");
            }
        }


        // =================================================
        // BRD: registration is confirmed by this POST.
        // =================================================
        var registration =
            new EventRegistration
            {
                EventId = dto.EventId,
                StudentId = studentId,
                EventSeatId = selectedSeatId,
                Status = EventRegistrationStatus.Confirmed,
                ExpiresAt = null,
                RegisteredAt = now
            };

        await _registrationRepository
            .AddAsync(registration);

        await _registrationRepository
            .SaveChangesAsync();

        await _notificationService.CreateAsync(
            new CreateNotificationDto
            {
                StudentId = studentId,
                Type = "EventRegistration",
                Title = "Event registration confirmed",
                Message = $"Your registration for {eventEntity.Title} is confirmed."
            });


        var createdRegistration =
            await _registrationRepository
                .GetByIdAsync(
                    registration.EventRegistrationId);

        if (createdRegistration is null)
        {
            throw new InvalidOperationException(
                "Registration was created but could not be retrieved.");
        }


        await transaction.CommitAsync();

        return MapToResponseDto(
            createdRegistration);
    }


    // =====================================================
    // CONFIRM HELD REGISTRATION
    // =====================================================
    public async Task ConfirmAsync(
        int registrationId)
    {
        var registration =
            await _registrationRepository
                .GetByIdAsync(
                    registrationId);

        if (registration is null)
        {
            throw new KeyNotFoundException(
                $"Registration with ID {registrationId} was not found.");
        }


        if (registration.Status ==
            EventRegistrationStatus.Confirmed)
        {
            return;
        }

        if (registration.Status !=
            EventRegistrationStatus.Held)
        {
            throw new InvalidOperationException(
                "Only a held registration can be confirmed.");
        }


        var now =
            DateTime.UtcNow;


        // =================================================
        // HOLD EXPIRED
        // =================================================
        if (!registration.ExpiresAt.HasValue ||
            registration.ExpiresAt.Value <= now)
        {
            registration.Status =
                EventRegistrationStatus.Expired;

            registration.ExpiresAt =
                null;

            _registrationRepository.Update(
                registration);

            await _registrationRepository
                .SaveChangesAsync();

            throw new InvalidOperationException(
                "The event registration hold has expired.");
        }


        var eventEntity =
            await _eventRepository.GetByIdAsync(
                registration.EventId);

        if (eventEntity is null)
        {
            throw new KeyNotFoundException(
                $"Event with ID {registration.EventId} was not found.");
        }


        if (!eventEntity.IsActive)
        {
            throw new InvalidOperationException(
                "This event is no longer active.");
        }


        if (!eventEntity.IsPublished)
        {
            throw new InvalidOperationException(
                "This event is no longer available for registration.");
        }


        if (eventEntity.EndAt <= now)
        {
            registration.Status =
                EventRegistrationStatus.Expired;

            registration.ExpiresAt =
                null;

            _registrationRepository.Update(
                registration);

            await _registrationRepository
                .SaveChangesAsync();

            throw new InvalidOperationException(
                "The event has already ended.");
        }


        // =================================================
        // RESERVED SEAT EXISTING 2-HOUR RULE
        //
        // General admission is not affected by this rule.
        // =================================================
        if (eventEntity.UsesReservedSeating)
        {
            var remainingTime =
                eventEntity.StartAt - now;

            if (remainingTime <=
                TimeSpan.FromHours(2))
            {
                registration.Status =
                    EventRegistrationStatus.Expired;

                registration.ExpiresAt =
                    null;

                _registrationRepository.Update(
                    registration);

                await _registrationRepository
                    .SaveChangesAsync();

                throw new InvalidOperationException(
                    "Seat confirmation is no longer allowed because the event is within two hours.");
            }


            if (!registration.EventSeatId.HasValue)
            {
                throw new InvalidOperationException(
                    "Reserved seating registration does not contain a seat.");
            }


            var seat =
                await _eventSeatRepository.GetByIdAsync(
                    registration.EventSeatId.Value);

            if (seat is null)
            {
                throw new KeyNotFoundException(
                    "The selected event seat was not found.");
            }

            if (!seat.IsActive)
            {
                throw new InvalidOperationException(
                    "The selected event seat is inactive.");
            }

            if (seat.EventId !=
                registration.EventId)
            {
                throw new InvalidOperationException(
                    "The selected seat does not belong to this event.");
            }
        }


        // =================================================
        // CONFIRM
        // =================================================
        registration.Status =
            EventRegistrationStatus.Confirmed;

        registration.ExpiresAt =
            null;

        _registrationRepository.Update(
            registration);

        await _registrationRepository
            .SaveChangesAsync();
    }


    // =====================================================
    // GET CURRENT STUDENT'S REGISTRATIONS
    // =====================================================
    public async Task<IEnumerable<MyEventRegistrationDto>>
        GetMyRegistrationsAsync(
            int studentId)
    {
        var registrations =
            (await _registrationRepository
                .GetByStudentIdAsync(
                    studentId))
            .ToList();


        var now =
            DateTime.UtcNow;

        var changed =
            false;


        foreach (var registration in registrations)
        {
            if (registration.Status ==
                    EventRegistrationStatus.Held
                &&
                registration.ExpiresAt.HasValue
                &&
                registration.ExpiresAt.Value <= now)
            {
                registration.Status =
                    EventRegistrationStatus.Expired;

                registration.ExpiresAt =
                    null;

                _registrationRepository.Update(
                    registration);

                changed =
                    true;
            }
        }


        if (changed)
        {
            await _registrationRepository
                .SaveChangesAsync();
        }


        return registrations
            .Select(MapToMyRegistrationDto)
            .ToList();
    }


    // =====================================================
    // GET REGISTRATION BY ID
    // =====================================================
    public async Task<EventRegistrationResponseDto?>
        GetByIdAsync(
            int registrationId)
    {
        var registration =
            await _registrationRepository
                .GetByIdAsync(
                    registrationId);

        if (registration is null)
        {
            return null;
        }


        // =================================================
        // LAZY EXPIRY SAFETY
        // =================================================
        if (registration.Status ==
                EventRegistrationStatus.Held
            &&
            registration.ExpiresAt.HasValue
            &&
            registration.ExpiresAt.Value <=
                DateTime.UtcNow)
        {
            registration.Status =
                EventRegistrationStatus.Expired;

            registration.ExpiresAt =
                null;

            _registrationRepository.Update(
                registration);

            await _registrationRepository
                .SaveChangesAsync();
        }


        return MapToResponseDto(
            registration);
    }


    // =====================================================
    // ADMIN - GET ALL
    // =====================================================
    public async Task<IEnumerable<EventRegistrationResponseDto>>
        GetAllAsync(
            RegistrationFilterDto filter)
    {
        var registrations =
            (await _registrationRepository
                .GetAllAsync(filter))
            .ToList();


        var now =
            DateTime.UtcNow;

        var changed =
            false;


        foreach (var registration in registrations)
        {
            if (registration.Status ==
                    EventRegistrationStatus.Held
                &&
                registration.ExpiresAt.HasValue
                &&
                registration.ExpiresAt.Value <= now)
            {
                registration.Status =
                    EventRegistrationStatus.Expired;

                registration.ExpiresAt =
                    null;

                _registrationRepository.Update(
                    registration);

                changed =
                    true;
            }
        }


        if (changed)
        {
            await _registrationRepository
                .SaveChangesAsync();
        }


        return registrations
            .Select(MapToResponseDto)
            .ToList();
    }


    // =====================================================
    // CANCEL REGISTRATION
    // =====================================================
    public async Task CancelAsync(
        int registrationId)
    {
        var registration =
            await _registrationRepository
                .GetByIdAsync(
                    registrationId);

        if (registration is null)
        {
            throw new KeyNotFoundException(
                $"Registration with ID {registrationId} was not found.");
        }


        // =================================================
        // HELD RECORD MAY ALREADY BE EXPIRED
        // =================================================
        if (registration.Status ==
                EventRegistrationStatus.Held
            &&
            registration.ExpiresAt.HasValue
            &&
            registration.ExpiresAt.Value <=
                DateTime.UtcNow)
        {
            registration.Status =
                EventRegistrationStatus.Expired;

            registration.ExpiresAt =
                null;

            _registrationRepository.Update(
                registration);

            await _registrationRepository
                .SaveChangesAsync();

            throw new InvalidOperationException(
                "An expired registration cannot be cancelled.");
        }


        if (registration.Status ==
            EventRegistrationStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "This registration is already cancelled.");
        }


        if (registration.Status ==
            EventRegistrationStatus.Expired)
        {
            throw new InvalidOperationException(
                "An expired registration cannot be cancelled.");
        }


        registration.Status =
            EventRegistrationStatus.Cancelled;

        registration.ExpiresAt =
            null;

        _registrationRepository.Update(
            registration);

        await _registrationRepository
            .SaveChangesAsync();
    }


    // =====================================================
    // MAP -> MY EVENT REGISTRATION
    // =====================================================
    private static MyEventRegistrationDto
        MapToMyRegistrationDto(
            EventRegistration registration)
    {
        return new MyEventRegistrationDto
        {
            EventRegistrationId =
                registration.EventRegistrationId,

            EventTitle =
                registration.Event?.Title
                ?? string.Empty,

            StartAt =
                registration.Event?.StartAt
                ?? default,

            VenueName =
                registration.Event?.Venue?.Name
                ?? string.Empty,

            Status =
                registration.Status,

            SeatNumber =
                registration.EventSeat?.SeatNumber
        };
    }


    // =====================================================
    // MAP -> EVENT REGISTRATION RESPONSE
    // =====================================================
    private static EventRegistrationResponseDto
        MapToResponseDto(
            EventRegistration registration)
    {
        return new EventRegistrationResponseDto
        {
            EventRegistrationId =
                registration.EventRegistrationId,

            EventId =
                registration.EventId,

            EventTitle =
                registration.Event?.Title
                ?? string.Empty,

            StudentId =
                registration.StudentId,

            StudentName =
                registration.Student?.FullName
                ?? string.Empty,

            EventSeatId =
                registration.EventSeatId,

            SeatNumber =
                registration.EventSeat?.SeatNumber,

            Status =
                registration.Status,

            ExpiresAt =
                registration.ExpiresAt,

            RegisteredAt =
                registration.RegisteredAt
        };
    }
}