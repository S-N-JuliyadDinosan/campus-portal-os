using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Modules.Certificates.DTOs;

public class CreateCertificateRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Certificate type is required.")]
    public int CertificateTypeId { get; set; }

    [MaxLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
    public string? Reason { get; set; }
}
