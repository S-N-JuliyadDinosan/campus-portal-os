using CampusServicesPortal.Modules.Events.DTOs.EventSeats;
using CampusServicesPortal.Modules.Events.Entities;
using CampusServicesPortal.Modules.Events.Interfaces.Repositories;
using CampusServicesPortal.Modules.Events.Interfaces.Services;

namespace CampusServicesPortal.Modules.Events.Services;

public sealed class EventSeatService : IEventSeatService
{
    private readonly IEventSeatRepository _eventSeatRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IEventRegistrationRepository _eventRegistrationRepository;

    public EventSeatService(
        IEventSeatRepository eventSeatRepository,
        IEventRepository eventRepository,
        IEventRegistrationRepository eventRegistrationRepository)
    {
        _eventSeatRepository = eventSeatRepository;
        _eventRepository = eventRepository;
        _eventRegistrationRepository = eventRegistrationRepository;
    }


    // =====================================================
    // GET SEATS FOR EVENT
    // RESERVED SEATING EVENT ONLY
    // =====================================================
    public async Task<IEnumerable<EventSeatResponseDto>>
        GetByEventIdAsync(
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

        if (!eventEntity.UsesReservedSeating)
        {
            throw new InvalidOperationException(
                "This event does not use reserved seating.");
        }

        var seats =
            await _eventSeatRepository
                .GetByEventIdAsync(
                    eventId);

        return seats
            .Select(MapToResponseDto)
            .ToList();
    }


    // =====================================================
    // GET SEAT AVAILABILITY
    // RESERVED SEATING EVENT ONLY
    // =====================================================
    public async Task<IEnumerable<SeatAvailabilityResponseDto>>
        GetAvailabilityAsync(
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

        if (!eventEntity.IsActive)
        {
            throw new InvalidOperationException(
                "This event is inactive.");
        }

        if (!eventEntity.UsesReservedSeating)
        {
            throw new InvalidOperationException(
                "Seat availability is only applicable to reserved-seating events.");
        }

        var seats =
            await _eventSeatRepository
                .GetByEventIdAsync(
                    eventId);

        var result =
            new List<SeatAvailabilityResponseDto>();


        foreach (var seat in seats)
        {
            // ---------------------------------------------
            // Inactive seat must NEVER be available.
            // ---------------------------------------------
            if (!seat.IsActive)
            {
                result.Add(
                    new SeatAvailabilityResponseDto
                    {
                        EventSeatId =
                            seat.EventSeatId,

                        SeatNumber =
                            seat.SeatNumber,

                        SectionName =
                            seat.SectionName,

                        RowLabel =
                            seat.RowLabel,

                        IsAvailable =
                            false
                    });

                continue;
            }


            var activeRegistration =
                await _eventRegistrationRepository
                    .GetActiveSeatRegistrationAsync(
                        seat.EventSeatId);


            result.Add(
                new SeatAvailabilityResponseDto
                {
                    EventSeatId =
                        seat.EventSeatId,

                    SeatNumber =
                        seat.SeatNumber,

                    SectionName =
                        seat.SectionName,

                    RowLabel =
                        seat.RowLabel,

                    IsAvailable =
                        activeRegistration is null
                });
        }


        return result;
    }


    // =====================================================
    // CREATE SEAT
    // RESERVED SEATING EVENT ONLY
    // =====================================================
    public async Task<EventSeatResponseDto> CreateAsync(
        int eventId,
        CreateEventSeatDto dto)
    {
        var eventEntity =
            await _eventRepository
                .GetByIdAsync(
                    eventId);

        if (eventEntity is null)
        {
            throw new KeyNotFoundException(
                $"Event with ID {eventId} was not found.");
        }

        if (!eventEntity.IsActive)
        {
            throw new InvalidOperationException(
                "Seats cannot be added to an inactive event.");
        }

        if (!eventEntity.UsesReservedSeating)
        {
            throw new InvalidOperationException(
                "Seats can only be added to a reserved-seating event.");
        }

        if (string.IsNullOrWhiteSpace(
                dto.SeatNumber))
        {
            throw new ArgumentException(
                "Seat number is required.");
        }


        var seatNumber =
            dto.SeatNumber.Trim();


        var seats =
            (await _eventSeatRepository
                .GetByEventIdAsync(
                    eventId))
            .ToList();


        // ---------------------------------------------
        // Prevent duplicate seat number.
        //
        // We check all seats, including inactive,
        // because a DB unique constraint may still
        // contain the soft-deleted seat row.
        // ---------------------------------------------
        var duplicateSeat =
            seats.Any(s =>
                s.SeatNumber.Equals(
                    seatNumber,
                    StringComparison.OrdinalIgnoreCase));

        if (duplicateSeat)
        {
            throw new InvalidOperationException(
                $"Seat '{seatNumber}' already exists for this event.");
        }


        // ---------------------------------------------
        // Only ACTIVE configured seats consume the
        // current seat configuration capacity.
        // ---------------------------------------------
        var activeSeatCount =
            seats.Count(s =>
                s.IsActive);

        if (activeSeatCount >=
            eventEntity.Capacity)
        {
            throw new InvalidOperationException(
                "The event has already reached its seat capacity.");
        }


        var seat =
            new EventSeat
            {
                EventId =
                    eventId,

                SeatNumber =
                    seatNumber,

                SectionName =
                    string.IsNullOrWhiteSpace(
                        dto.SectionName)
                        ? null
                        : dto.SectionName.Trim(),

                RowLabel =
                    string.IsNullOrWhiteSpace(
                        dto.RowLabel)
                        ? null
                        : dto.RowLabel.Trim(),

                IsActive =
                    true
            };


        await _eventSeatRepository
            .AddAsync(
                seat);

        await _eventSeatRepository
            .SaveChangesAsync();


        return MapToResponseDto(
            seat);
    }


    // =====================================================
    // UPDATE SEAT
    //
    // IMPORTANT:
    // Cannot deactivate a currently held/confirmed seat.
    // =====================================================
    public async Task UpdateAsync(
        int seatId,
        UpdateEventSeatDto dto)
    {
        var seat =
            await _eventSeatRepository
                .GetByIdAsync(
                    seatId);

        if (seat is null)
        {
            throw new KeyNotFoundException(
                $"Event seat with ID {seatId} was not found.");
        }


        var eventEntity =
            await _eventRepository
                .GetByIdAsync(
                    seat.EventId);

        if (eventEntity is null)
        {
            throw new KeyNotFoundException(
                $"Event with ID {seat.EventId} was not found.");
        }

        if (!eventEntity.UsesReservedSeating)
        {
            throw new InvalidOperationException(
                "This event does not use reserved seating.");
        }


        if (string.IsNullOrWhiteSpace(
                dto.SeatNumber))
        {
            throw new ArgumentException(
                "Seat number is required.");
        }


        // =================================================
        // DEACTIVATION PROTECTION
        //
        // This closes the loophole where admin could use
        // PUT isActive=false instead of DELETE.
        // =================================================
        if (seat.IsActive &&
            !dto.IsActive)
        {
            var activeRegistration =
                await _eventRegistrationRepository
                    .GetActiveSeatRegistrationAsync(
                        seatId);

            if (activeRegistration is not null)
            {
                throw new InvalidOperationException(
                    "This seat cannot be deactivated because it is currently held or confirmed.");
            }
        }


        var seatNumber =
            dto.SeatNumber.Trim();


        var seats =
            await _eventSeatRepository
                .GetByEventIdAsync(
                    seat.EventId);


        var duplicateSeat =
            seats.Any(s =>
                s.EventSeatId != seatId
                &&
                s.SeatNumber.Equals(
                    seatNumber,
                    StringComparison.OrdinalIgnoreCase));

        if (duplicateSeat)
        {
            throw new InvalidOperationException(
                $"Seat '{seatNumber}' already exists for this event.");
        }


        seat.SeatNumber =
            seatNumber;

        seat.SectionName =
            string.IsNullOrWhiteSpace(
                dto.SectionName)
                ? null
                : dto.SectionName.Trim();

        seat.RowLabel =
            string.IsNullOrWhiteSpace(
                dto.RowLabel)
                ? null
                : dto.RowLabel.Trim();

        seat.IsActive =
            dto.IsActive;


        _eventSeatRepository.Update(
            seat);

        await _eventSeatRepository
            .SaveChangesAsync();
    }


    // =====================================================
    // DELETE / DEACTIVATE SEAT
    //
    // Held or Confirmed registration blocks removal.
    // =====================================================
    public async Task DeleteAsync(
        int seatId)
    {
        var seat =
            await _eventSeatRepository
                .GetByIdAsync(
                    seatId);

        if (seat is null)
        {
            throw new KeyNotFoundException(
                $"Event seat with ID {seatId} was not found.");
        }


        var eventEntity =
            await _eventRepository
                .GetByIdAsync(
                    seat.EventId);

        if (eventEntity is null)
        {
            throw new KeyNotFoundException(
                $"Event with ID {seat.EventId} was not found.");
        }

        if (!eventEntity.UsesReservedSeating)
        {
            throw new InvalidOperationException(
                "This event does not use reserved seating.");
        }


        if (!seat.IsActive)
        {
            return;
        }


        var activeRegistration =
            await _eventRegistrationRepository
                .GetActiveSeatRegistrationAsync(
                    seatId);

        if (activeRegistration is not null)
        {
            throw new InvalidOperationException(
                "This seat cannot be deactivated because it is currently held or confirmed.");
        }


        // Soft delete
        seat.IsActive =
            false;


        _eventSeatRepository.Update(
            seat);

        await _eventSeatRepository
            .SaveChangesAsync();
    }


    // =====================================================
    // MAP ENTITY -> DTO
    // =====================================================
    private static EventSeatResponseDto
        MapToResponseDto(
            EventSeat seat)
    {
        return new EventSeatResponseDto
        {
            EventSeatId =
                seat.EventSeatId,

            EventId =
                seat.EventId,

            SeatNumber =
                seat.SeatNumber,

            SectionName =
                seat.SectionName,

            RowLabel =
                seat.RowLabel,

            IsActive =
                seat.IsActive,

            CreatedAt =
                seat.CreatedAt,

            UpdatedAt =
                seat.UpdatedAt
        };
    }
}