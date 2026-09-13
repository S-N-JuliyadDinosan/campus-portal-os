namespace CampusServicesPortal.Modules.Complaints.DTOs;

public sealed class ComplaintCategoryResponseDto
{
    public int ComplaintCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}