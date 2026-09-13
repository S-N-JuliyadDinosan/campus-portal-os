using CampusServicesPortal.Modules.Students.DTOs;

namespace CampusServicesPortal.Modules.Students.Interfaces;

public interface IFacultyService
{
    Task<IReadOnlyCollection<FacultyResponse>> ListActiveAsync(CancellationToken cancellationToken = default);
    Task<FacultyResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<FacultyResponse> CreateAsync(CreateFacultyRequest request, CancellationToken cancellationToken = default);
    Task<FacultyResponse> UpdateAsync(int id, UpdateFacultyRequest request, CancellationToken cancellationToken = default);
    Task<FacultyDeleteResponse> DeleteOrDeactivateAsync(int id, CancellationToken cancellationToken = default);
}
