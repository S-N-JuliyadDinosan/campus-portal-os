using CampusServicesPortal.Common.Entities;

namespace CampusServicesPortal.Modules.Complaints.Entities;

public sealed class ComplaintCategory : AuditableEntity
{
    public int ComplaintCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Complaint> Complaints { get; set; } = [];
}
