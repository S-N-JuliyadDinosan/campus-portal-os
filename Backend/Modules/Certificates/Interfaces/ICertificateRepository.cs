using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Modules.Certificates.Entities;

namespace CampusServicesPortal.Modules.Certificates.Interfaces;

public interface ICertificateRepository
{
    Task<List<CertificateType>> GetCertificateTypesAsync(bool activeOnly = false);
    Task<CertificateType?> GetCertificateTypeByIdAsync(int id, bool tracking = false);
    Task<bool> CertificateTypeNameExistsAsync(string name, int? excludeId = null);
    Task<bool> CertificateTypeHasRequestsAsync(int certificateTypeId);
    Task<CertificateType> CreateCertificateTypeAsync(CertificateType certificateType);
    Task<CertificateType> UpdateCertificateTypeAsync(CertificateType certificateType);
    Task DeleteCertificateTypeAsync(CertificateType certificateType);

    Task<List<CertificateRequest>> GetAllRequestsAsync(
        CertificateRequestStatus? status,
        int page = 1);

    Task<CertificateRequest?> GetRequestByIdAsync(int id, bool tracking = false);
    Task<List<CertificateRequest>> GetRequestsByStudentIdAsync(int studentId);
    Task<bool> HasPendingRequestAsync(int studentId, int certificateTypeId);
    Task<CertificateRequest> CreateRequestAsync(CertificateRequest request);
    Task<CertificateRequest> UpdateRequestAsync(CertificateRequest request);
}
