using CampusServicesPortal.Common.Entities;
using CampusServicesPortal.Common.Enums;

namespace CampusServicesPortal.Modules.Labs.Entities;

public sealed class Lab : AuditableEntity
{
    public int LabId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public LabType LabType { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<LabTimeSlot> TimeSlots { get; set; } = [];
    public ICollection<LabSeat> Seats { get; set; } = [];
    public ICollection<LabBooking> Bookings { get; set; } = [];
}
