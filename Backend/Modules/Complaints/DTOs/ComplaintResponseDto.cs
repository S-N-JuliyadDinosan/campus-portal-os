using CampusServicesPortal.Common.Enums;

namespace CampusServicesPortal.Modules.Complaints.DTOs;

public sealed class ComplaintResponseDto
{
    public int ComplaintId { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string StudentIndexNumber { get; set; } = string.Empty;

    public int ComplaintCategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ComplaintStatus Status { get; set; }

    public string? ResolutionNote { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public int? StatusChangedByUserId { get; set; }
}
