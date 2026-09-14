using CampusService.Modules.Events.DTOs.Venues;
using CampusServicesPortal.Modules.Events.DTOs.Venues;
using CampusServicesPortal.Modules.Events.Entities;
using CampusServicesPortal.Modules.Events.Interfaces.Repositories;
using CampusServicesPortal.Modules.Events.Interfaces.Services;

namespace CampusServicesPortal.Modules.Events.Services;

public sealed class VenueService : IVenueService
{
    private readonly IVenueRepository _venueRepository;
    private readonly IEventRepository _eventRepository;

    public VenueService(
        IVenueRepository venueRepository,
        IEventRepository eventRepository)
    {
        _venueRepository = venueRepository;
        _eventRepository = eventRepository;
    }


    // =====================================================
    // GET ALL VENUES
    // =====================================================
    public async Task<IEnumerable<VenueResponseDto>>
        GetAllAsync()
    {
        var venues =
            await _venueRepository.GetAllAsync();

        return venues
            .Select(MapToResponseDto)
            .ToList();
    }


    // =====================================================
    // GET VENUE BY ID
    // =====================================================
    public async Task<VenueResponseDto?>
        GetByIdAsync(
            int venueId)
    {
        var venue =
            await _venueRepository.GetByIdAsync(
                venueId);

        if (venue is null)
        {
            return null;
        }

        return MapToResponseDto(
            venue);
    }


    // =====================================================
    // CHECK VENUE AVAILABILITY
    // =====================================================
    public async Task<VenueAvailabilityResponseDto>
        CheckAvailabilityAsync(
            int venueId,
            DateTime from,
            DateTime to)
    {
        if (from >= to)
        {
            throw new ArgumentException(
                "The 'from' time must be before the 'to' time.");
        }

        var venue =
            await _venueRepository.GetByIdAsync(
                venueId);

        if (venue is null)
        {
            throw new KeyNotFoundException(
                $"Venue with ID {venueId} was not found.");
        }

        var isAvailable =
            await _venueRepository
                .IsVenueAvailableAsync(
                    venueId,
                    from,
                    to);

        return new VenueAvailabilityResponseDto
        {
            VenueId =
                venue.VenueId,

            VenueName =
                venue.Name,

            IsAvailable =
                isAvailable,

            Message =
                isAvailable
                    ? "Venue is available."
                    : "Venue is already booked for the selected time."
        };
    }


    // =====================================================
    // CREATE VENUE
    // =====================================================
    public async Task<VenueResponseDto>
        CreateAsync(
            CreateVenueDto dto)
    {
        ValidateVenueData(
            dto.Name,
            dto.Capacity);

        var venue =
            new Venue
            {
                Name =
                    dto.Name.Trim(),

                VenueType =
                    dto.VenueType,

                Location =
                    string.IsNullOrWhiteSpace(
                        dto.Location)
                        ? string.Empty
                        : dto.Location.Trim(),

                Capacity =
                    dto.Capacity,

                IsActive =
                    true
            };

        await _venueRepository.AddAsync(
            venue);

        await _venueRepository
            .SaveChangesAsync();

        return MapToResponseDto(
            venue);
    }


    // =====================================================
    // UPDATE VENUE
    // =====================================================
    public async Task UpdateAsync(
        int venueId,
        UpdateVenueDto dto)
    {
        ValidateVenueData(
            dto.Name,
            dto.Capacity);

        var venue =
            await _venueRepository.GetByIdAsync(
                venueId);

        if (venue is null)
        {
            throw new KeyNotFoundException(
                $"Venue with ID {venueId} was not found.");
        }


        // =================================================
        // HIGHEST ACTIVE EVENT CAPACITY
        // =================================================
        var maxActiveEventCapacity =
            await _eventRepository
                .GetMaxActiveEventCapacityByVenueAsync(
                    venueId);


        // =================================================
        // VENUE CAPACITY PROTECTION
        // =================================================
        if (dto.Capacity <
            maxActiveEventCapacity)
        {
            throw new InvalidOperationException(
                $"Venue capacity cannot be reduced to {dto.Capacity} because an active event requires capacity {maxActiveEventCapacity}.");
        }


        // =================================================
        // DEACTIVATION PROTECTION
        // =================================================
        if (venue.IsActive &&
            !dto.IsActive &&
            maxActiveEventCapacity > 0)
        {
            throw new InvalidOperationException(
                "This venue cannot be deactivated while active events are assigned to it.");
        }


        venue.Name =
            dto.Name.Trim();

        venue.VenueType =
            dto.VenueType;

        venue.Location =
            string.IsNullOrWhiteSpace(
                dto.Location)
                ? string.Empty
                : dto.Location.Trim();

        venue.Capacity =
            dto.Capacity;

        venue.IsActive =
            dto.IsActive;


        _venueRepository.Update(
            venue);

        await _venueRepository
            .SaveChangesAsync();
    }


    // =====================================================
    // DELETE VENUE
    // =====================================================
    public async Task DeleteAsync(
        int venueId)
    {
        var venue =
            await _venueRepository.GetByIdAsync(
                venueId);

        if (venue is null)
        {
            throw new KeyNotFoundException(
                $"Venue with ID {venueId} was not found.");
        }


        var hasEvents =
            await _eventRepository
                .HasEventsForVenueAsync(
                    venueId);

        if (hasEvents)
        {
            throw new InvalidOperationException(
                "This venue cannot be deleted because events are still assigned to it. Delete those events first.");
        }

        _venueRepository.Delete(venue);

        await _venueRepository
            .SaveChangesAsync();
    }


    // =====================================================
    // COMMON VALIDATION
    // =====================================================
    private static void ValidateVenueData(
        string name,
        int capacity)
    {
        if (string.IsNullOrWhiteSpace(
                name))
        {
            throw new ArgumentException(
                "Venue name is required.");
        }

        if (capacity <= 0)
        {
            throw new ArgumentException(
                "Venue capacity must be greater than zero.");
        }
    }


    // =====================================================
    // MAP ENTITY -> DTO
    // =====================================================
    private static VenueResponseDto
        MapToResponseDto(
            Venue venue)
    {
        return new VenueResponseDto
        {
            VenueId =
                venue.VenueId,

            Name =
                venue.Name,

            VenueType =
                venue.VenueType,

            Location =
                venue.Location,

            Capacity =
                venue.Capacity,

            IsActive =
                venue.IsActive,

            CreatedAt =
                venue.CreatedAt,

            UpdatedAt =
                venue.UpdatedAt
        };
    }
}
