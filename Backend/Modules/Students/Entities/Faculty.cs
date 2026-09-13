using CampusServicesPortal.Common.Entities;

namespace CampusServicesPortal.Modules.Students.Entities;

public sealed class Faculty : AuditableEntity
{
    public int FacultyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<Student> Students { get; set; } = [];
    public ICollection<StudentMaster> StudentMasterRecords { get; set; } = [];
}
