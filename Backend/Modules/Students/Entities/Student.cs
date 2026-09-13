using CampusServicesPortal.Common.Entities;
using CampusServicesPortal.Modules.Identity.Entities;

namespace CampusServicesPortal.Modules.Students.Entities;

public sealed class Student : AuditableEntity
{
    public int StudentId { get; set; }
    public int UserId { get; set; }
    public int StudentMasterId { get; set; }
    public int FacultyId { get; set; }
    public string IndexNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateTime? DeactivatedAt { get; set; }

    public User User { get; set; } = null!;
    public StudentMaster StudentMaster { get; set; } = null!;
    public Faculty Faculty { get; set; } = null!;
}
