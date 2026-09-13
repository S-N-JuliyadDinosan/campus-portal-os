namespace CampusServicesPortal.Modules.Identity.Interfaces;

public interface IEmailService
{
    Task SendEmailVerificationAsync(
        string email,
        string rawToken,
        CancellationToken cancellationToken = default);

    Task SendPasswordResetAsync(
        string email,
        string rawToken,
        CancellationToken cancellationToken = default);
}
