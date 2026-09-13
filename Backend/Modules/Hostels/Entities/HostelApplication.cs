using CampusServicesPortal.Common.Entities;
using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Modules.Identity.Entities;
using CampusServicesPortal.Modules.Students.Entities;

namespace CampusServicesPortal.Modules.Hostels.Entities;

public sealed class HostelApplication : AuditableEntity
{
    public int HostelApplicationId { get; set; }
    public int StudentId { get; set; }
    public int PreferredHostelId { get; set; }
    public int? AssignedRoomId { get; set; }
    public int? ReviewedByUserId { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;
    public string? SpecialRequirements { get; set; }
    public HostelApplicationStatus Status { get; set; } =
        HostelApplicationStatus.Pending;
    public string? ReviewNote { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public Student Student { get; set; } = null!;
    public Hostel PreferredHostel { get; set; } = null!;
    public Room? AssignedRoom { get; set; }
    public User? ReviewedByUser { get; set; }
}
