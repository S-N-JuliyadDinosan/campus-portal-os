using CampusServicesPortal.Common.Enums;

namespace CampusServicesPortal.Modules.Complaints.DTOs;

public sealed class ComplaintFilterDto
{
    public ComplaintStatus? Status { get; set; }

    public int? CategoryId { get; set; }

    public int? StudentId { get; set; }

    public int Page { get; set; } = 1;
}