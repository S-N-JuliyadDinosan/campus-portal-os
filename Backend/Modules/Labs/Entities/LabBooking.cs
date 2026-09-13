using CampusServicesPortal.Common.Entities;
using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Modules.Students.Entities;

namespace CampusServicesPortal.Modules.Labs.Entities;

public sealed class LabBooking : AuditableEntity
{
    public int LabBookingId { get; set; }
    public int StudentId { get; set; }
    public int LabId { get; set; }
    public int LabTimeSlotId { get; set; }
    public int? LabSeatId { get; set; }
    public DateOnly BookingDate { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Held;
    public DateTime? ExpiresAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public Student Student { get; set; } = null!;
    public Lab Lab { get; set; } = null!;
    public LabTimeSlot LabTimeSlot { get; set; } = null!;
    public LabSeat? LabSeat { get; set; }
}
