using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Identity.Entities;
using CampusServicesPortal.Modules.Identity.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Identity.Repositories;

public sealed class UserRepository(ApplicationDbContext dbContext)
    : IUserRepository
{
    public Task<User?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default) =>
        dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(
                user => user.Email == email,
                cancellationToken);
}
