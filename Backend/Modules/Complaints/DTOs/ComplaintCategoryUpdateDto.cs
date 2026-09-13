namespace CampusServicesPortal.Modules.Complaints.DTOs;

public sealed class ComplaintCategoryUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}