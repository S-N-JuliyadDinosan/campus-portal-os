namespace CampusServicesPortal.Modules.Complaints.DTOs;

public sealed class CreateComplaintDto
{
    public int ComplaintCategoryId { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsAnonymous { get; set; }
}