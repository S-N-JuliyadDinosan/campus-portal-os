namespace CampusServicesPortal.Modules.Complaints.DTOs;

public sealed class ComplaintCategoryCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}