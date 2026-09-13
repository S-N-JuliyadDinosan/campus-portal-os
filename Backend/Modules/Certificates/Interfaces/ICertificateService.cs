using CampusServicesPortal.Modules.Certificates.DTOs;

namespace CampusServicesPortal.Modules.Certificates.Interfaces;

public interface ICertificateService
{
    // =============================================
    // Certificate Types
    // =============================================

    Task<List<CertificateTypeDto>>
        GetCertificateTypesAsync();

    Task<CertificateTypeDto?>
        GetCertificateTypeByIdAsync(int id);

    Task<CertificateTypeDto>
        CreateCertificateTypeAsync(
            CreateCertificateTypeDto dto);

    Task<CertificateTypeDto>
        UpdateCertificateTypeAsync(
            int id,
            UpdateCertificateTypeDto dto);

    Task DeleteOrDeactivateCertificateTypeAsync(
        int id);


    // =============================================
    // Certificate Requests
    // =============================================

    Task<List<CertificateRequestDto>>
        GetAllRequestsAsync(
            string? status,
            int page = 1);

    Task<CertificateRequestDto?>
        GetRequestByIdAsync(int id);

    Task<List<CertificateRequestDto>>
        GetMyRequestsAsync();

    Task<List<CertificateRequestDto>>
        GetRequestsByStudentIdAsync(
            int studentId);

    Task<CertificateRequestDto>
        CreateRequestAsync(
            CreateCertificateRequestDto dto);

    Task<CertificateRequestDto>
        UpdateRequestStatusAsync(
            int id,
            UpdateCertificateRequestDto dto);
}