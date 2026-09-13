using Microsoft.AspNetCore.Http;
using CampusServicesPortal.Common.Pagination;
using CampusServicesPortal.Modules.Students.DTOs;

namespace CampusServicesPortal.Modules.Students.Interfaces;

public interface IStudentMasterService
{
    Task<StudentMasterCheckResponse> CheckAsync(string indexNumber, CancellationToken cancellationToken = default);
    Task<PagedResult<StudentMasterResponse>> SearchAsync(string? search, int? facultyId, int? intakeYear, bool? isActive, int page, CancellationToken cancellationToken = default);
    Task<StudentMasterResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<StudentMasterResponse> CreateAsync(CreateStudentMasterRequest request, CancellationToken cancellationToken = default);
    Task<StudentMasterResponse> UpdateAsync(int id, UpdateStudentMasterRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<StudentMasterImportResponse> ImportCsvAsync(IFormFile file, CancellationToken cancellationToken = default);
}
