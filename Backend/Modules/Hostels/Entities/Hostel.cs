using CampusServicesPortal.Common.Entities;

namespace CampusServicesPortal.Modules.Hostels.Entities;

public sealed class Hostel : AuditableEntity
{
    public int HostelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Room> Rooms { get; set; } = [];
    public ICollection<HostelApplication> Applications { get; set; } = [];
}
