using CampusServicesPortal.Common.Pagination;
using CampusServicesPortal.Modules.Students.DTOs;

namespace CampusServicesPortal.Modules.Students.Interfaces;

public interface IStudentService
{
    Task<StudentRegistrationResponse> RegisterAsync(StudentRegisterRequest request, CancellationToken cancellationToken = default);
    Task<StudentProfileResponse> GetMyProfileAsync(int studentId, CancellationToken cancellationToken = default);
    Task<StudentProfileResponse> UpdateMyProfileAsync(int studentId, UpdateMyStudentRequest request, CancellationToken cancellationToken = default);
    Task<StudentActivitySummaryResponse> GetActivitySummaryAsync(int studentId, CancellationToken cancellationToken = default);
    Task<AdminStudentDetailResponse> GetByIdAsync(int studentId, CancellationToken cancellationToken = default);
    Task<PagedResult<StudentListItemResponse>> SearchAsync(
        string? search,
        int? facultyId,
        string? faculty,
        bool? isActive,
        int page,
        CancellationToken cancellationToken = default);
    Task<StudentProfileResponse> AdminUpdateAsync(int studentId, AdminUpdateStudentRequest request, CancellationToken cancellationToken = default);
    Task<DeactivationCheckResponse> CheckDeactivationAsync(int studentId, CancellationToken cancellationToken = default);
    Task DeactivateAsync(int studentId, CancellationToken cancellationToken = default);
    Task ReactivateAsync(int studentId, CancellationToken cancellationToken = default);
}
