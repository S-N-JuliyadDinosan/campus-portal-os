using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Modules.Labs.DTOs;
using CampusServicesPortal.Modules.Labs.Entities;
using CampusServicesPortal.Modules.Labs.Interfaces;

namespace CampusServicesPortal.Modules.Labs.Services;

public sealed class LabSeatService : ILabSeatService
{
    private readonly ILabSeatRepository _seatRepository;
    private readonly ILabRepository _labRepository;
    private readonly ILabTimeSlotRepository _timeSlotRepository;
    private readonly ILabBookingRepository _bookingRepository;

    public LabSeatService(
        ILabSeatRepository seatRepository,
        ILabRepository labRepository,
        ILabTimeSlotRepository timeSlotRepository,
        ILabBookingRepository bookingRepository)
    {
        _seatRepository = seatRepository;
        _labRepository = labRepository;
        _timeSlotRepository = timeSlotRepository;
        _bookingRepository = bookingRepository;
    }


    // =====================================================
    // GET ALL SEATS FOR A LAB
    // =====================================================
    public async Task<List<LabSeatResponse>> GetByLabIdAsync(
        int labId)
    {
        var lab =
            await _labRepository.GetByIdAsync(labId);

        if (lab == null)
        {
            throw new KeyNotFoundException(
                "Lab not found.");
        }

        if (lab.LabType != LabType.Computer)
        {
            throw new InvalidOperationException(
                "Science Labs do not use individual seats.");
        }

        var seats =
            await _seatRepository.GetByLabIdAsync(
                labId);

        return seats
            .Select(MapToResponse)
            .ToList();
    }


    // =====================================================
    // GET SEAT BY ID
    // =====================================================
    public async Task<LabSeatResponse?> GetByIdAsync(
        int labSeatId)
    {
        var seat =
            await _seatRepository.GetByIdAsync(
                labSeatId);

        return seat == null
            ? null
            : MapToResponse(seat);
    }


    // =====================================================
    // CREATE SEAT
    // COMPUTER LAB ONLY
    // =====================================================
    public async Task<LabSeatResponse> CreateAsync(
        int labId,
        CreateLabSeatRequest request)
    {
        var lab =
            await _labRepository.GetByIdAsync(
                labId);

        if (lab == null)
        {
            throw new KeyNotFoundException(
                "Lab not found.");
        }

        if (!lab.IsActive)
        {
            throw new InvalidOperationException(
                "Cannot add a seat to an inactive lab.");
        }

        if (lab.LabType != LabType.Computer)
        {
            throw new InvalidOperationException(
                "Seats can only be created for Computer Labs.");
        }

        if (string.IsNullOrWhiteSpace(
                request.SeatNumber))
        {
            throw new ArgumentException(
                "Seat number is required.");
        }

        var seatNumber =
            request.SeatNumber.Trim();

        var exists =
            await _seatRepository
                .SeatNumberExistsAsync(
                    labId,
                    seatNumber);

        if (exists)
        {
            throw new InvalidOperationException(
                "Seat number already exists in this lab.");
        }

        var seat = new LabSeat
        {
            LabId = labId,
            SeatNumber = seatNumber,
            IsActive = true
        };

        seat =
            await _seatRepository.AddAsync(
                seat);

        return MapToResponse(seat);
    }


    // =====================================================
    // UPDATE SEAT
    // COMPUTER LAB ONLY
    // =====================================================
    public async Task<bool> UpdateAsync(
        int labSeatId,
        UpdateLabSeatRequest request)
    {
        var seat =
            await _seatRepository.GetByIdAsync(
                labSeatId);

        if (seat == null)
        {
            return false;
        }

        var lab =
            await _labRepository.GetByIdAsync(
                seat.LabId);

        if (lab == null)
        {
            throw new KeyNotFoundException(
                "Lab not found.");
        }

        if (lab.LabType != LabType.Computer)
        {
            throw new InvalidOperationException(
                "Science Labs do not use individual seats.");
        }

        if (string.IsNullOrWhiteSpace(
                request.SeatNumber))
        {
            throw new ArgumentException(
                "Seat number is required.");
        }

        var seatNumber =
            request.SeatNumber.Trim();

        var exists =
            await _seatRepository
                .SeatNumberExistsAsync(
                    seat.LabId,
                    seatNumber,
                    labSeatId);

        if (exists)
        {
            throw new InvalidOperationException(
                "Seat number already exists in this lab.");
        }


        // -------------------------------------------------
        // Prevent deactivation when future booking exists
        // -------------------------------------------------
        if (seat.IsActive &&
            !request.IsActive)
        {
            var hasFutureBooking =
                await _bookingRepository
                    .HasFutureActiveSeatBookingAsync(
                        labSeatId);

            if (hasFutureBooking)
            {
                throw new InvalidOperationException(
                    "This seat cannot be deactivated because it has a future active booking.");
            }
        }


        seat.SeatNumber =
            seatNumber;

        seat.IsActive =
            request.IsActive;

        await _seatRepository.UpdateAsync(
            seat);

        return true;
    }


    // =====================================================
    // DELETE / DEACTIVATE SEAT
    //
    // Physical delete is avoided so historical bookings
    // can continue referencing the seat.
    // =====================================================
    public async Task<bool> DeleteAsync(
        int labSeatId)
    {
        var seat =
            await _seatRepository.GetByIdAsync(
                labSeatId);

        if (seat == null)
        {
            return false;
        }

        var lab =
            await _labRepository.GetByIdAsync(
                seat.LabId);

        if (lab == null)
        {
            throw new KeyNotFoundException(
                "Lab not found.");
        }

        if (lab.LabType != LabType.Computer)
        {
            throw new InvalidOperationException(
                "Science Labs do not use individual seats.");
        }

        if (!seat.IsActive)
        {
            return true;
        }


        var hasFutureBooking =
            await _bookingRepository
                .HasFutureActiveSeatBookingAsync(
                    labSeatId);

        if (hasFutureBooking)
        {
            throw new InvalidOperationException(
                "This seat cannot be deactivated because it has a future active booking.");
        }


        seat.IsActive =
            false;

        await _seatRepository.UpdateAsync(
            seat);

        return true;
    }


    // =====================================================
    // GET AVAILABLE COMPUTER LAB SEATS
    // =====================================================
    public async Task<List<LabSeatResponse>>
        GetAvailableSeatsAsync(
            int labId,
            int labTimeSlotId,
            DateOnly bookingDate)
    {
        var lab =
            await _labRepository.GetByIdAsync(
                labId);

        if (lab == null)
        {
            throw new KeyNotFoundException(
                "Lab not found.");
        }

        if (!lab.IsActive)
        {
            throw new InvalidOperationException(
                "Selected lab is inactive.");
        }

        if (lab.LabType != LabType.Computer)
        {
            throw new InvalidOperationException(
                "Seat availability is only available for Computer Labs.");
        }


        var timeSlot =
            await _timeSlotRepository.GetByIdAsync(
                labTimeSlotId);

        if (timeSlot == null)
        {
            throw new KeyNotFoundException(
                "Lab time slot not found.");
        }

        if (timeSlot.LabId != labId)
        {
            throw new InvalidOperationException(
                "Selected time slot does not belong to this lab.");
        }

        if (!timeSlot.IsActive)
        {
            throw new InvalidOperationException(
                "Selected time slot is inactive.");
        }

        if (timeSlot.DayOfWeek !=
            bookingDate.DayOfWeek)
        {
            throw new InvalidOperationException(
                "Selected time slot is not available on this date.");
        }


        var seats =
            await _seatRepository.GetByLabIdAsync(
                labId);

        var availableSeats =
            new List<LabSeatResponse>();


        foreach (var seat in seats)
        {
            if (!seat.IsActive)
            {
                continue;
            }

            var booked =
                await _bookingRepository
                    .IsSeatBookedAsync(
                        seat.LabSeatId,
                        labTimeSlotId,
                        bookingDate);

            if (!booked)
            {
                availableSeats.Add(
                    MapToResponse(seat));
            }
        }


        return availableSeats;
    }


    // =====================================================
    // MAP ENTITY -> RESPONSE
    // =====================================================
    private static LabSeatResponse MapToResponse(
        LabSeat seat)
    {
        return new LabSeatResponse
        {
            LabSeatId =
                seat.LabSeatId,

            LabId =
                seat.LabId,

            SeatNumber =
                seat.SeatNumber,

            IsActive =
                seat.IsActive
        };
    }
}