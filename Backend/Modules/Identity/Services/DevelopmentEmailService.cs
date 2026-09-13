using CampusServicesPortal.Modules.Identity.Interfaces;

namespace CampusServicesPortal.Modules.Identity.Services;

public sealed class DevelopmentEmailService(
    ILogger<DevelopmentEmailService> logger) : IEmailService
{
    public Task SendEmailVerificationAsync(
        string email,
        string rawToken,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "DEV EMAIL -> Verification token for {Email}: {Token}",
            email,
            rawToken);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetAsync(
        string email,
        string rawToken,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "DEV EMAIL -> Password reset token for {Email}: {Token}",
            email,
            rawToken);
        return Task.CompletedTask;
    }
}
