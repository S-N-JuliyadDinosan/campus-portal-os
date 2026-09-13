using CampusServicesPortal.Common.Entities;
using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Modules.Identity.Entities;
using CampusServicesPortal.Modules.Students.Entities;

namespace CampusServicesPortal.Modules.Certificates.Entities;

public class CertificateRequest : AuditableEntity
{
    public int CertificateRequestId { get; set; }

    public int CertificateTypeId { get; set; }

    public int StudentId { get; set; }

    public int? ReviewedByUserId { get; set; }

    public string? Reason { get; set; }

    public CertificateRequestStatus Status { get; set; }
        = CertificateRequestStatus.Pending;

    public string? ReviewNote { get; set; }

    public DateTime? ReviewedAt { get; set; }

    // Navigation properties

    public CertificateType CertificateType { get; set; } = null!;

    public Student Student { get; set; } = null!;

    public User? ReviewedByUser { get; set; }
}