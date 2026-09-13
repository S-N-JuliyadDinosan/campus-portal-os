using CampusServicesPortal.Common.Entities;

namespace CampusServicesPortal.Modules.Students.Entities;

public sealed class StudentMaster : AuditableEntity
{
    public int StudentMasterId { get; set; }
    public string IndexNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? OfficialEmail { get; set; }
    public int FacultyId { get; set; }
    public int IntakeYear { get; set; }
    public bool IsActive { get; set; } = true;

    public Faculty Faculty { get; set; } = null!;
    public Student? Student { get; set; }
}
