using CampusServicesPortal.Common.Entities;

namespace CampusServicesPortal.Modules.Hostels.Entities;

public sealed class Room : AuditableEntity
{
    public int RoomId { get; set; }
    public int HostelId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool IsActive { get; set; } = true;

    public Hostel Hostel { get; set; } = null!;
    public ICollection<HostelApplication> AssignedApplications { get; set; } = [];
}
