using CampusServicesPortal.Modules.Students.DTOs;

namespace CampusServicesPortal.Modules.Students.Interfaces;

public interface IAdminAccountService
{
    Task<AdminAccountResponse> CreateAsync(CreateAdminRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AdminAccountResponse>> ListAsync(CancellationToken cancellationToken = default);
    Task<AdminAccountResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AdminAccountResponse> UpdateAsync(int id, UpdateAdminRequest request, CancellationToken cancellationToken = default);
    Task DeactivateAsync(int id, int currentUserId, CancellationToken cancellationToken = default);
    Task ReactivateAsync(int id, CancellationToken cancellationToken = default);
}
