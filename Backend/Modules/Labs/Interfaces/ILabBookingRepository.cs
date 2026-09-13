using CampusServicesPortal.Modules.Labs.Entities;

namespace CampusServicesPortal.Modules.Labs.Interfaces;

public interface ILabBookingRepository
{
    Task<LabBooking?> GetByIdAsync(
        int labBookingId);

    Task<List<LabBooking>> GetByStudentIdAsync(
        int studentId);

    Task<List<LabBooking>> GetAllAsync(
        int? labId = null,
        int? studentId = null,
        string? status = null,
        DateOnly? date = null,
        int page = 1);

    Task<LabBooking> AddAsync(
        LabBooking booking);

    Task UpdateAsync(
        LabBooking booking);

    Task<bool> HasStudentBookingAsync(
        int studentId,
        int labTimeSlotId,
        DateOnly bookingDate);

    Task<bool> IsTimeSlotBookedAsync(
        int labId,
        int labTimeSlotId,
        DateOnly bookingDate);

    Task<bool> IsSeatBookedAsync(
        int labSeatId,
        int labTimeSlotId,
        DateOnly bookingDate,
        int? excludeBookingId = null);

    Task<int> GetActiveBookingCountAsync(
        int labId,
        int labTimeSlotId,
        DateOnly bookingDate);

    // =====================================================
    // CHECK WHETHER A SEAT HAS A FUTURE ACTIVE BOOKING
    //
    // Used before deactivating/deleting a Computer Lab seat.
    //
    // Active means:
    // - Confirmed
    // OR
    // - Held and not yet expired
    // =====================================================
    Task<bool> HasFutureActiveSeatBookingAsync(
        int labSeatId);
}