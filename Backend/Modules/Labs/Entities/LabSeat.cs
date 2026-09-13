using CampusServicesPortal.Common.Entities;

namespace CampusServicesPortal.Modules.Labs.Entities;

public sealed class LabSeat : AuditableEntity
{
    public int LabSeatId { get; set; }
    public int LabId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Lab Lab { get; set; } = null!;
    public ICollection<LabBooking> Bookings { get; set; } = [];
}
