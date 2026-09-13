using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Modules.Certificates.DTOs;

public class UpdateCertificateTypeDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }
}
