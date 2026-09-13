using CampusServicesPortal.Modules.Identity.Entities;

namespace CampusServicesPortal.Modules.Identity.Interfaces;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);
}
