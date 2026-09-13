using CampusServicesPortal.Modules.Identity.Entities;
using CampusServicesPortal.Modules.Identity.Interfaces;
using CampusServicesPortal.Modules.Identity.Repositories;
using CampusServicesPortal.Modules.Identity.Services;
using Microsoft.AspNetCore.Identity;

namespace CampusServicesPortal.Modules.Identity;

public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        // services.AddScoped<IEmailService, DevelopmentEmailService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        return services;
    }
}
