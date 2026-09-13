using CampusServicesPortal.Common.Enums;

namespace CampusServicesPortal.Modules.Complaints.DTOs;

public sealed class UpdateComplaintStatusDto
{
    public ComplaintStatus Status { get; set; }
    public string? ResolutionNote { get; set; }
}