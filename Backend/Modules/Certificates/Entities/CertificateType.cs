namespace CampusServicesPortal.Modules.Certificates.Entities;

public class CertificateType
{
    public int CertificateTypeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<CertificateRequest> Requests { get; set; }
        = new List<CertificateRequest>();
}