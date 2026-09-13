namespace CampusServicesPortal.Modules.Certificates.DTOs;

public class CertificateTypeDto
{
    public int CertificateTypeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}