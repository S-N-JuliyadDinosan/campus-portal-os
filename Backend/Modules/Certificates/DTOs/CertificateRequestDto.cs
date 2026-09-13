namespace CampusServicesPortal.Modules.Certificates.DTOs;

public class CertificateRequestDto
{
    public int CertificateRequestId { get; set; }

    public int CertificateTypeId { get; set; }

    public string CertificateTypeName { get; set; }
        = string.Empty;

    public int StudentId { get; set; }

    public string StudentIndexNumber { get; set; }
        = string.Empty;

    public string StudentName { get; set; }
        = string.Empty;

    public int? ReviewedByUserId { get; set; }

    public string? Reason { get; set; }

    public string Status { get; set; }
        = "Pending";

    public string? ReviewNote { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }
}
