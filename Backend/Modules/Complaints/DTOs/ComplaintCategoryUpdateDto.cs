namespace CampusServicesPortal.Modules.Complaints.DTOs;

using System.ComponentModel.DataAnnotations;

public sealed class ComplaintCategoryUpdateDto
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
