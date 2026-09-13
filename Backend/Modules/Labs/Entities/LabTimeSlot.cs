using CampusServicesPortal.Common.Entities;

namespace CampusServicesPortal.Modules.Labs.Entities;

public sealed class LabTimeSlot : AuditableEntity
{
    public int LabTimeSlotId { get; set; }
    public int LabId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsActive { get; set; } = true;

    public Lab Lab { get; set; } = null!;
    public ICollection<LabBooking> Bookings { get; set; } = [];
}
