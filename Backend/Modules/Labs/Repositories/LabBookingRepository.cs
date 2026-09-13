using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Labs.Entities;
using CampusServicesPortal.Modules.Labs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Labs.Repositories;

public sealed class LabBookingRepository
    : ILabBookingRepository
{
    private readonly ApplicationDbContext _context;

    public LabBookingRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }


    // =====================================================
    // GET BOOKING BY ID
    // =====================================================
    public async Task<LabBooking?> GetByIdAsync(
        int labBookingId)
    {
        return await _context.LabBookings
            .Include(x => x.Lab)
            .Include(x => x.LabTimeSlot)
            .Include(x => x.LabSeat)
            .FirstOrDefaultAsync(
                x =>
                    x.LabBookingId ==
                    labBookingId);
    }


    // =====================================================
    // GET BOOKINGS BY STUDENT
    // =====================================================
    public async Task<List<LabBooking>>
        GetByStudentIdAsync(
            int studentId)
    {
        return await _context.LabBookings
            .AsNoTracking()
            .Include(x => x.Lab)
            .Include(x => x.LabTimeSlot)
            .Include(x => x.LabSeat)
            .Where(x =>
                x.StudentId == studentId)
            .OrderByDescending(x =>
                x.BookingDate)
            .ThenByDescending(x =>
                x.CreatedAt)
            .ToListAsync();
    }


    // =====================================================
    // ADMIN - GET ALL BOOKINGS
    // =====================================================
    public async Task<List<LabBooking>> GetAllAsync(
        int? labId = null,
        int? studentId = null,
        string? status = null,
        DateOnly? date = null,
        int page = 1)
    {
        const int pageSize = 20;

        if (page < 1)
        {
            page = 1;
        }

        var query =
            _context.LabBookings
                .AsNoTracking()
                .Include(x => x.Lab)
                .Include(x => x.LabTimeSlot)
                .Include(x => x.LabSeat)
                .AsQueryable();

        if (labId.HasValue)
        {
            query = query.Where(
                x =>
                    x.LabId ==
                    labId.Value);
        }

        if (studentId.HasValue)
        {
            query = query.Where(
                x =>
                    x.StudentId ==
                    studentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<ReservationStatus>(
                    status.Trim(),
                    true,
                    out var parsedStatus))
            {
                query = query.Where(
                    x =>
                        x.Status ==
                        parsedStatus);
            }
        }

        if (date.HasValue)
        {
            query = query.Where(
                x =>
                    x.BookingDate ==
                    date.Value);
        }

        return await query
            .OrderByDescending(
                x => x.BookingDate)
            .ThenByDescending(
                x => x.CreatedAt)
            .Skip(
                (page - 1) *
                pageSize)
            .Take(pageSize)
            .ToListAsync();
    }


    // =====================================================
    // ADD BOOKING
    // =====================================================
    public async Task<LabBooking> AddAsync(
        LabBooking booking)
    {
        _context.LabBookings.Add(
            booking);

        await _context.SaveChangesAsync();

        return booking;
    }


    // =====================================================
    // UPDATE BOOKING
    // =====================================================
    public async Task UpdateAsync(
        LabBooking booking)
    {
        _context.LabBookings.Update(
            booking);

        await _context.SaveChangesAsync();
    }


    // =====================================================
    // CHECK STUDENT DUPLICATE BOOKING
    // =====================================================
    public async Task<bool> HasStudentBookingAsync(
        int studentId,
        int labTimeSlotId,
        DateOnly bookingDate)
    {
        var now =
            DateTime.UtcNow;

        return await _context.LabBookings
            .AnyAsync(x =>
                x.StudentId ==
                    studentId
                &&
                x.LabTimeSlotId ==
                    labTimeSlotId
                &&
                x.BookingDate ==
                    bookingDate
                &&
                (
                    x.Status ==
                        ReservationStatus.Confirmed
                    ||
                    (
                        x.Status ==
                            ReservationStatus.Held
                        &&
                        (
                            x.ExpiresAt == null
                            ||
                            x.ExpiresAt > now
                        )
                    )
                ));
    }


    // =====================================================
    // BRD RULE: A LAB TIME SLOT CANNOT BE DOUBLE-BOOKED.
    // =====================================================
    public async Task<bool> IsTimeSlotBookedAsync(
        int labId,
        int labTimeSlotId,
        DateOnly bookingDate)
    {
        var now = DateTime.UtcNow;

        return await _context.LabBookings.AnyAsync(x =>
            x.LabId == labId &&
            x.LabTimeSlotId == labTimeSlotId &&
            x.BookingDate == bookingDate &&
            (
                x.Status == ReservationStatus.Confirmed ||
                (x.Status == ReservationStatus.Held &&
                 (x.ExpiresAt == null || x.ExpiresAt > now))
            ));
    }


    // =====================================================
    // CHECK WHETHER SPECIFIC SEAT IS BOOKED / HELD
    // =====================================================
    public async Task<bool> IsSeatBookedAsync(
        int labSeatId,
        int labTimeSlotId,
        DateOnly bookingDate,
        int? excludeBookingId = null)
    {
        var now =
            DateTime.UtcNow;

        var query =
            _context.LabBookings
                .Where(x =>
                    x.LabSeatId ==
                        labSeatId
                    &&
                    x.LabTimeSlotId ==
                        labTimeSlotId
                    &&
                    x.BookingDate ==
                        bookingDate
                    &&
                    (
                        x.Status ==
                            ReservationStatus.Confirmed
                        ||
                        (
                            x.Status ==
                                ReservationStatus.Held
                            &&
                            (
                                x.ExpiresAt == null
                                ||
                                x.ExpiresAt > now
                            )
                        )
                    ));

        if (excludeBookingId.HasValue)
        {
            query = query.Where(
                x =>
                    x.LabBookingId !=
                    excludeBookingId.Value);
        }

        return await query.AnyAsync();
    }


    // =====================================================
    // GET ACTIVE BOOKING COUNT
    // Used mainly for Science Lab capacity.
    // =====================================================
    public async Task<int> GetActiveBookingCountAsync(
        int labId,
        int labTimeSlotId,
        DateOnly bookingDate)
    {
        var now =
            DateTime.UtcNow;

        return await _context.LabBookings
            .CountAsync(x =>
                x.LabId ==
                    labId
                &&
                x.LabTimeSlotId ==
                    labTimeSlotId
                &&
                x.BookingDate ==
                    bookingDate
                &&
                (
                    x.Status ==
                        ReservationStatus.Confirmed
                    ||
                    (
                        x.Status ==
                            ReservationStatus.Held
                        &&
                        (
                            x.ExpiresAt == null
                            ||
                            x.ExpiresAt > now
                        )
                    )
                ));
    }


    // =====================================================
    // CHECK FUTURE ACTIVE BOOKING FOR A SEAT
    //
    // Prevents administrator from deactivating a seat
    // that is still used by an active future booking.
    //
    // Active:
    // - Confirmed
    // - Held and not expired
    //
    // Cancelled / Expired do not block the seat.
    // =====================================================
    public async Task<bool> HasFutureActiveSeatBookingAsync(
        int labSeatId)
    {
        var now =
            DateTime.UtcNow;

        var today =
            DateOnly.FromDateTime(now);

        return await _context.LabBookings
            .AnyAsync(x =>
                x.LabSeatId ==
                    labSeatId
                &&
                x.BookingDate >= today
                &&
                (
                    x.Status ==
                        ReservationStatus.Confirmed
                    ||
                    (
                        x.Status ==
                            ReservationStatus.Held
                        &&
                        (
                            x.ExpiresAt == null
                            ||
                            x.ExpiresAt > now
                        )
                    )
                ));
    }
}