using CampusServicesPortal.Common.Exceptions;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Identity.Entities;
using CampusServicesPortal.Modules.Students.DTOs;
using CampusServicesPortal.Modules.Students.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Students.Services;

public sealed class AdminAccountService(
    ApplicationDbContext dbContext,
    IPasswordHasher<User> passwordHasher) : IAdminAccountService
{
    public async Task<AdminAccountResponse> CreateAsync(
        CreateAdminRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await dbContext.Users.AnyAsync(x => x.Email == email, cancellationToken))
            throw new BusinessRuleException("Email is already registered.");

        var user = new User
        {
            Email = email,
            Role = "Admin",
            EmailVerified = true,
            IsActive = true
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(user);
    }

    public async Task<IReadOnlyCollection<AdminAccountResponse>> ListAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Users
            .AsNoTracking()
            .Where(x => x.Role == "Admin")
            .OrderBy(x => x.Email)
            .Select(x => new AdminAccountResponse(
                x.UserId, x.Email, x.IsActive, x.EmailVerified, x.CreatedAt, x.UpdatedAt))
            .ToListAsync(cancellationToken);

    public async Task<AdminAccountResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var user = await GetAdminAsync(id, cancellationToken);
        return Map(user);
    }

    public async Task<AdminAccountResponse> UpdateAsync(
        int id,
        UpdateAdminRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await GetAdminAsync(id, cancellationToken);
        var email = request.Email.Trim().ToLowerInvariant();

        if (await dbContext.Users.AnyAsync(x => x.Email == email && x.UserId != id, cancellationToken))
            throw new BusinessRuleException("Email is already in use.");

        user.Email = email;
        user.SecurityStamp = Guid.NewGuid().ToString("N");
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(user);
    }

    public async Task DeactivateAsync(
        int id,
        int currentUserId,
        CancellationToken cancellationToken = default)
    {
        if (id == currentUserId)
            throw new BusinessRuleException("You cannot deactivate your own administrator account.");

        var user = await GetAdminAsync(id, cancellationToken);
        if (!user.IsActive) return;

        user.IsActive = false;
        user.SecurityStamp = Guid.NewGuid().ToString("N");
        var now = DateTime.UtcNow;
        var tokens = await dbContext.RefreshTokens
            .Where(x => x.UserId == id && x.RevokedAt == null && x.ExpiresAt > now)
            .ToListAsync(cancellationToken);
        foreach (var token in tokens)
            token.RevokedAt = now;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ReactivateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var user = await GetAdminAsync(id, cancellationToken);
        user.IsActive = true;
        user.SecurityStamp = Guid.NewGuid().ToString("N");
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<User> GetAdminAsync(int id, CancellationToken cancellationToken) =>
        await dbContext.Users.SingleOrDefaultAsync(
            x => x.UserId == id && x.Role == "Admin",
            cancellationToken)
        ?? throw new NotFoundException("Administrator account not found.");

    private static AdminAccountResponse Map(User user) =>
        new(user.UserId, user.Email, user.IsActive, user.EmailVerified, user.CreatedAt, user.UpdatedAt);
}
