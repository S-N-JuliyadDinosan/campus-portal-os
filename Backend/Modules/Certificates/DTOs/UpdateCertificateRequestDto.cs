namespace CampusServicesPortal.Modules.Certificates.DTOs;

public class UpdateCertificateRequestDto
{
    public string Status { get; set; }
        = string.Empty;

    public string? ReviewNote { get; set; }
}