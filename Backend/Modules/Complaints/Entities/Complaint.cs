using CampusServicesPortal.Common.Entities;
using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Modules.Identity.Entities;
using CampusServicesPortal.Modules.Students.Entities;

namespace CampusServicesPortal.Modules.Complaints.Entities;

public sealed class Complaint : AuditableEntity
{
    public int ComplaintId { get; set; }
    public int StudentId { get; set; }
    public int ComplaintCategoryId { get; set; }

    public bool IsAnonymous { get; set; }
    public int? StatusChangedByUserId { get; set; }
    public string Description { get; set; } = string.Empty;
    public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;
    public string? ResolutionNote { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public Student Student { get; set; } = null!;
    public ComplaintCategory ComplaintCategory { get; set; } = null!;
    public User? StatusChangedByUser { get; set; }
}
