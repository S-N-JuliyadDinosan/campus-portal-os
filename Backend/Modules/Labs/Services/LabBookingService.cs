using System.Data;
using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Common.Security;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Labs.DTOs;
using CampusServicesPortal.Modules.Labs.Entities;
using CampusServicesPortal.Modules.Labs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Labs.Services;

public sealed class LabBookingService
    : ILabBookingService
{
    private readonly ILabBookingRepository
        _bookingRepository;

    private readonly ILabRepository
        _labRepository;

    private readonly ILabTimeSlotRepository
        _timeSlotRepository;

    private readonly ILabSeatRepository
        _seatRepository;

    private readonly ICurrentUserService
        _currentUserService;

    private readonly ApplicationDbContext
        _dbContext;


    public LabBookingService(
        ILabBookingRepository bookingRepository,
        ILabRepository labRepository,
        ILabTimeSlotRepository timeSlotRepository,
        ILabSeatRepository seatRepository,
        ICurrentUserService currentUserService,
        ApplicationDbContext dbContext)
    {
        _bookingRepository =
            bookingRepository;

        _labRepository =
            labRepository;

        _timeSlotRepository =
            timeSlotRepository;

        _seatRepository =
            seatRepository;

        _currentUserService =
            currentUserService;

        _dbContext =
            dbContext;
    }


    // =========================================================
    // STUDENT - CREATE LAB BOOKING
    // =========================================================
    public async Task<LabBookingResponse> CreateAsync(
        CreateLabBookingRequest request)
    {
        var studentId =
            GetCurrentStudentId();


        if (request.LabId <= 0)
        {
            throw new ArgumentException(
                "Lab is required.");
        }


        if (request.LabTimeSlotId <= 0)
        {
            throw new ArgumentException(
                "Lab time slot is required.");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (request.BookingDate < today)
        {
            throw new ArgumentException(
                "Lab booking date cannot be in the past.");
        }


        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    IsolationLevel.Serializable);

        try
        {
            // -------------------------------------------------
            // LAB VALIDATION
            // -------------------------------------------------
            var lab =
                await _labRepository
                    .GetByIdAsync(
                        request.LabId);

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


            // -------------------------------------------------
            // TIME SLOT VALIDATION
            // -------------------------------------------------
            var timeSlot =
                await _timeSlotRepository
                    .GetByIdAsync(
                        request.LabTimeSlotId);

            if (timeSlot == null)
            {
                throw new KeyNotFoundException(
                    "Lab time slot not found.");
            }


            if (timeSlot.LabId !=
                request.LabId)
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
                request.BookingDate.DayOfWeek)
            {
                throw new InvalidOperationException(
                    "Selected time slot is not available on this date.");
            }


            // -------------------------------------------------
            // DUPLICATE STUDENT BOOKING CHECK
            //
            // IMPORTANT:
            // request.StudentId is ignored.
            // Student identity comes from JWT.
            // -------------------------------------------------
            var alreadyBooked =
                await _bookingRepository
                    .HasStudentBookingAsync(
                        studentId,
                        request.LabTimeSlotId,
                        request.BookingDate);

            if (alreadyBooked)
            {
                throw new InvalidOperationException(
                    "You already have an active booking for this time slot.");
            }

            var slotAlreadyBooked =
                await _bookingRepository.IsTimeSlotBookedAsync(
                    request.LabId,
                    request.LabTimeSlotId,
                    request.BookingDate);

            if (slotAlreadyBooked)
            {
                throw new InvalidOperationException(
                    "This lab time slot has already been booked.");
            }


            LabSeat? selectedSeat = null;


            // =================================================
            // COMPUTER LAB
            //
            // Specific seat is mandatory.
            // =================================================
            if (lab.LabType ==
                LabType.Computer)
            {
                if (!request.LabSeatId.HasValue ||
                    request.LabSeatId.Value <= 0)
                {
                    throw new InvalidOperationException(
                        "A specific seat must be selected for a Computer Lab booking.");
                }


                selectedSeat =
                    await _seatRepository
                        .GetByIdAsync(
                            request.LabSeatId.Value);

                if (selectedSeat == null)
                {
                    throw new KeyNotFoundException(
                        "Lab seat not found.");
                }


                if (selectedSeat.LabId !=
                    request.LabId)
                {
                    throw new InvalidOperationException(
                        "Selected seat does not belong to this lab.");
                }


                if (!selectedSeat.IsActive)
                {
                    throw new InvalidOperationException(
                        "Selected seat is inactive.");
                }


                var seatBooked =
                    await _bookingRepository
                        .IsSeatBookedAsync(
                            selectedSeat.LabSeatId,
                            request.LabTimeSlotId,
                            request.BookingDate);

                if (seatBooked)
                {
                    throw new InvalidOperationException(
                        "Selected seat is already booked or currently held by another student.");
                }
            }


            // =================================================
            // SCIENCE LAB
            //
            // No seat should be selected.
            // Capacity is checked at lab level.
            // =================================================
            else if (lab.LabType ==
                LabType.Science)
            {
                if (request.LabSeatId.HasValue)
                {
                    throw new InvalidOperationException(
                        "A specific seat cannot be selected for a Science Lab booking.");
                }


                var activeBookingCount =
                    await _bookingRepository
                        .GetActiveBookingCountAsync(
                            request.LabId,
                            request.LabTimeSlotId,
                            request.BookingDate);

                if (activeBookingCount >=
                    lab.Capacity)
                {
                    throw new InvalidOperationException(
                        "No booking capacity is available for this Science Lab time slot.");
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "The selected lab has an invalid lab type.");
            }


            // -------------------------------------------------
            // BRD: POST /api/lab-bookings reserves immediately.
            // -------------------------------------------------
            var now = DateTime.UtcNow;

            var booking =
                new LabBooking
                {
                    StudentId = studentId,
                    LabId = request.LabId,
                    LabTimeSlotId = request.LabTimeSlotId,
                    LabSeatId = selectedSeat?.LabSeatId,
                    BookingDate = request.BookingDate,
                    Status = ReservationStatus.Confirmed,
                    ExpiresAt = null,
                    ConfirmedAt = now,
                    CancelledAt = null
                };

            booking =
                await _bookingRepository
                    .AddAsync(booking);


            await transaction.CommitAsync();


            // Navigation values for response.
            booking.Lab =
                lab;

            booking.LabTimeSlot =
                timeSlot;

            booking.LabSeat =
                selectedSeat;


            return MapToResponse(
                booking);
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }


    // =========================================================
    // GET BOOKING BY ID
    // =========================================================
    public async Task<LabBookingResponse?>
        GetByIdAsync(
            int labBookingId)
    {
        var booking =
            await _bookingRepository
                .GetByIdAsync(
                    labBookingId);

        if (booking == null)
        {
            return null;
        }


        await ExpireIfRequiredAsync(
            booking);


        return MapToResponse(
            booking);
    }


    // =========================================================
    // GET BOOKINGS BY STUDENT
    // =========================================================
    public async Task<List<LabBookingResponse>>
        GetByStudentIdAsync(
            int studentId)
    {
        var bookings =
            await _bookingRepository
                .GetByStudentIdAsync(
                    studentId);


        return bookings
            .Select(MapToResponse)
            .ToList();
    }


    // =========================================================
    // ADMIN - GET ALL BOOKINGS
    // =========================================================
    public async Task<List<LabBookingResponse>>
        GetAllAsync(
            int? labId = null,
            int? studentId = null,
            string? status = null,
            DateOnly? date = null,
            int page = 1)
    {
        if (page < 1)
        {
            throw new ArgumentException(
                "Page must be greater than zero.");
        }


        var bookings =
            await _bookingRepository
                .GetAllAsync(
                    labId,
                    studentId,
                    status,
                    date,
                    page);


        return bookings
            .Select(MapToResponse)
            .ToList();
    }


    // =========================================================
    // STUDENT - CONFIRM HELD BOOKING
    // =========================================================
    public async Task<bool> ConfirmAsync(
        int labBookingId)
    {
        var currentStudentId =
            GetCurrentStudentId();


        var booking =
            await _bookingRepository
                .GetByIdAsync(
                    labBookingId);

        if (booking == null)
        {
            return false;
        }


        if (booking.StudentId !=
            currentStudentId)
        {
            throw new UnauthorizedAccessException(
                "You cannot confirm another student's lab booking.");
        }


        if (booking.Status ==
            ReservationStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Cancelled booking cannot be confirmed.");
        }


        if (booking.Status ==
            ReservationStatus.Expired)
        {
            throw new InvalidOperationException(
                "Expired booking cannot be confirmed.");
        }


        if (booking.Status ==
            ReservationStatus.Confirmed)
        {
            return true;
        }


        if (booking.Status !=
            ReservationStatus.Held)
        {
            throw new InvalidOperationException(
                "Only a held booking can be confirmed.");
        }


        if (!booking.ExpiresAt.HasValue ||
            booking.ExpiresAt.Value <=
            DateTime.UtcNow)
        {
            booking.Status =
                ReservationStatus.Expired;

            booking.ExpiresAt =
                null;

            await _bookingRepository
                .UpdateAsync(booking);


            throw new InvalidOperationException(
                "Booking hold has expired.");
        }


        booking.Status =
            ReservationStatus.Confirmed;

        booking.ConfirmedAt =
            DateTime.UtcNow;

        booking.ExpiresAt =
            null;


        await _bookingRepository
            .UpdateAsync(booking);


        return true;
    }


    // =========================================================
    // STUDENT - CANCEL OWN BOOKING
    // =========================================================
    public async Task<bool> CancelAsync(
        int labBookingId)
    {
        var currentStudentId =
            GetCurrentStudentId();


        var booking =
            await _bookingRepository
                .GetByIdAsync(
                    labBookingId);

        if (booking == null)
        {
            return false;
        }


        if (booking.StudentId !=
            currentStudentId)
        {
            throw new UnauthorizedAccessException(
                "You cannot cancel another student's lab booking.");
        }


        if (booking.Status ==
            ReservationStatus.Cancelled)
        {
            return true;
        }


        if (booking.Status ==
            ReservationStatus.Expired)
        {
            throw new InvalidOperationException(
                "Expired booking cannot be cancelled.");
        }


        if (booking.Status ==
                ReservationStatus.Held
            &&
            booking.ExpiresAt.HasValue
            &&
            booking.ExpiresAt.Value <=
                DateTime.UtcNow)
        {
            booking.Status =
                ReservationStatus.Expired;

            booking.ExpiresAt =
                null;

            await _bookingRepository
                .UpdateAsync(booking);


            throw new InvalidOperationException(
                "Booking hold has already expired.");
        }


        booking.Status =
            ReservationStatus.Cancelled;

        booking.CancelledAt =
            DateTime.UtcNow;

        booking.ExpiresAt =
            null;


        await _bookingRepository
            .UpdateAsync(booking);


        return true;
    }


    // =========================================================
    // EXPIRE HELD BOOKING WHEN REQUIRED
    //
    // Background hold service will also handle expiry globally.
    // This protects GET-by-ID from returning a stale Held status.
    // =========================================================
    private async Task ExpireIfRequiredAsync(
        LabBooking booking)
    {
        if (booking.Status !=
            ReservationStatus.Held)
        {
            return;
        }


        if (!booking.ExpiresAt.HasValue)
        {
            return;
        }


        if (booking.ExpiresAt.Value >
            DateTime.UtcNow)
        {
            return;
        }


        booking.Status =
            ReservationStatus.Expired;

        booking.ExpiresAt =
            null;


        await _bookingRepository
            .UpdateAsync(booking);
    }


    // =========================================================
    // CURRENT STUDENT FROM JWT
    // =========================================================
    private int GetCurrentStudentId()
    {
        if (!_currentUserService
            .StudentId
            .HasValue)
        {
            throw new UnauthorizedAccessException(
                "The current account is not associated with a student.");
        }


        return _currentUserService
            .StudentId
            .Value;
    }


    // =========================================================
    // MAP ENTITY -> RESPONSE
    // =========================================================
    private static LabBookingResponse MapToResponse(
        LabBooking booking)
    {
        return new LabBookingResponse
        {
            LabBookingId =
                booking.LabBookingId,

            StudentId =
                booking.StudentId,

            StudentName =
                booking.Student?.FullName ?? string.Empty,

            StudentIndexNumber =
                booking.Student?.IndexNumber ?? string.Empty,

            LabId =
                booking.LabId,

            LabName =
                booking.Lab?.Name,

            LabTimeSlotId =
                booking.LabTimeSlotId,

            DayOfWeek =
                booking.LabTimeSlot
                    ?.DayOfWeek
                    .ToString(),

            StartTime =
                booking.LabTimeSlot
                    ?.StartTime
                    .ToTimeSpan()
                ?? TimeSpan.Zero,

            EndTime =
                booking.LabTimeSlot
                    ?.EndTime
                    .ToTimeSpan()
                ?? TimeSpan.Zero,

            LabSeatId =
                booking.LabSeatId,

            SeatNumber =
                booking.LabSeat
                    ?.SeatNumber,

            BookingDate =
                booking.BookingDate,

            Status =
                booking.Status
                    .ToString(),

            ExpiresAt =
                booking.ExpiresAt,

            CreatedAt =
                booking.CreatedAt
        };
    }
}
