using CampusServicesPortal.Modules.Identity.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CampusServicesPortal.Modules.Identity.Services;

public sealed class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(
        IConfiguration configuration,
        ILogger<SmtpEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // =========================================================
    // EMAIL VERIFICATION
    // =========================================================
    public async Task SendEmailVerificationAsync(
        string email,
        string rawToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "Email address is required.",
                nameof(email));
        }

        if (string.IsNullOrWhiteSpace(rawToken))
        {
            throw new ArgumentException(
                "Verification token is required.",
                nameof(rawToken));
        }

        var subject =
            "Campus Services Portal - Verify Your Email";

        var htmlBody = $"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8">
            </head>

            <body style="
                font-family: Arial, Helvetica, sans-serif;
                background-color: #f5f7fb;
                margin: 0;
                padding: 30px;
            ">

                <div style="
                    max-width: 600px;
                    margin: auto;
                    background-color: white;
                    border-radius: 12px;
                    padding: 30px;
                    box-shadow: 0 4px 12px rgba(0,0,0,0.08);
                ">

                    <h2 style="
                        color: #4338ca;
                        margin-top: 0;
                    ">
                        Campus Services Portal
                    </h2>

                    <p>
                        Your student account has been created successfully.
                    </p>

                    <p>
                        Please use the verification token below
                        to verify your email address.
                    </p>

                    <div style="
                        margin: 25px 0;
                        padding: 18px;
                        background-color: #eef2ff;
                        border: 1px solid #c7d2fe;
                        border-radius: 8px;
                        text-align: center;
                        font-size: 22px;
                        font-weight: bold;
                        letter-spacing: 2px;
                        color: #312e81;
                        word-break: break-all;
                    ">
                        {rawToken}
                    </div>

                    <p>
                        This verification token is valid for
                        <strong>24 hours</strong>.
                    </p>

                    <p>
                        If you did not create this account,
                        you can safely ignore this email.
                    </p>

                    <hr style="
                        border: none;
                        border-top: 1px solid #e5e7eb;
                        margin: 25px 0;
                    ">

                    <p style="
                        color: #6b7280;
                        font-size: 14px;
                    ">
                        Regards,<br>
                        Campus Services Portal
                    </p>

                </div>

            </body>
            </html>
            """;

        await SendEmailAsync(
            email,
            subject,
            htmlBody,
            cancellationToken);

        _logger.LogInformation(
            "Email verification message sent to {Email}",
            email);
    }

    // =========================================================
    // PASSWORD RESET
    // =========================================================
    public async Task SendPasswordResetAsync(
        string email,
        string rawToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "Email address is required.",
                nameof(email));
        }

        if (string.IsNullOrWhiteSpace(rawToken))
        {
            throw new ArgumentException(
                "Password reset token is required.",
                nameof(rawToken));
        }

        var subject =
            "Campus Services Portal - Password Reset";

        var htmlBody = $"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8">
            </head>

            <body style="
                font-family: Arial, Helvetica, sans-serif;
                background-color: #f5f7fb;
                margin: 0;
                padding: 30px;
            ">

                <div style="
                    max-width: 600px;
                    margin: auto;
                    background-color: white;
                    border-radius: 12px;
                    padding: 30px;
                    box-shadow: 0 4px 12px rgba(0,0,0,0.08);
                ">

                    <h2 style="
                        color: #4338ca;
                        margin-top: 0;
                    ">
                        Password Reset
                    </h2>

                    <p>
                        We received a request to reset
                        your Campus Services Portal password.
                    </p>

                    <p>
                        Use the token below to reset your password.
                    </p>

                    <div style="
                        margin: 25px 0;
                        padding: 18px;
                        background-color: #eef2ff;
                        border: 1px solid #c7d2fe;
                        border-radius: 8px;
                        text-align: center;
                        font-size: 22px;
                        font-weight: bold;
                        letter-spacing: 2px;
                        color: #312e81;
                        word-break: break-all;
                    ">
                        {rawToken}
                    </div>

                    <p>
                        This password reset token is valid for
                        <strong>1 hour</strong>.
                    </p>

                    <p>
                        If you did not request a password reset,
                        you can safely ignore this email.
                    </p>

                    <hr style="
                        border: none;
                        border-top: 1px solid #e5e7eb;
                        margin: 25px 0;
                    ">

                    <p style="
                        color: #6b7280;
                        font-size: 14px;
                    ">
                        Regards,<br>
                        Campus Services Portal
                    </p>

                </div>

            </body>
            </html>
            """;

        await SendEmailAsync(
            email,
            subject,
            htmlBody,
            cancellationToken);

        _logger.LogInformation(
            "Password reset message sent to {Email}",
            email);
    }

    // =========================================================
    // COMMON SMTP SEND METHOD
    // =========================================================
    private async Task SendEmailAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken)
    {
        var section =
            _configuration.GetSection("Email");

        var host =
            section["Host"];

        var portText =
            section["Port"];

        var username =
            section["Username"];

        var password =
            section["Password"];

        var fromEmail =
            section["FromEmail"];

        var fromName =
            section["FromName"];

        var enableSslText =
            section["EnableSsl"];

        // =====================================================
        // CONFIG VALIDATION
        // =====================================================

        if (string.IsNullOrWhiteSpace(host))
        {
            throw new InvalidOperationException(
                "Email Host is not configured in appsettings.json.");
        }

        if (!int.TryParse(portText, out var port))
        {
            throw new InvalidOperationException(
                "Email Port is invalid in appsettings.json.");
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            throw new InvalidOperationException(
                "Email Username is not configured in appsettings.json.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Email Password is not configured in appsettings.json.");
        }

        if (string.IsNullOrWhiteSpace(fromEmail))
        {
            fromEmail = username;
        }

        if (string.IsNullOrWhiteSpace(fromName))
        {
            fromName =
                "Campus Services Portal";
        }

        var enableSsl = true;

        if (!string.IsNullOrWhiteSpace(enableSslText))
        {
            bool.TryParse(
                enableSslText,
                out enableSsl);
        }

        // Detect placeholder values early
        if (
            username.Contains(
                "YOUR_GMAIL",
                StringComparison.OrdinalIgnoreCase)
            ||
            password.Contains(
                "YOUR_GMAIL_APP_PASSWORD",
                StringComparison.OrdinalIgnoreCase)
        )
        {
            throw new InvalidOperationException(
                "Gmail SMTP credentials are still placeholders. " +
                "Configure a real Gmail address and Gmail App Password.");
        }

        // =====================================================
        // CREATE EMAIL
        // =====================================================

        var message =
            new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                fromName,
                fromEmail));

        message.To.Add(
            MailboxAddress.Parse(
                toEmail));

        message.Subject =
            subject;

        var bodyBuilder =
            new BodyBuilder
            {
                HtmlBody = htmlBody
            };

        message.Body =
            bodyBuilder.ToMessageBody();

        // =====================================================
        // SMTP SEND
        // =====================================================

        using var smtp =
            new SmtpClient();

        try
        {
            var socketOption =
                enableSsl
                    ? SecureSocketOptions.StartTls
                    : SecureSocketOptions.None;

            await smtp.ConnectAsync(
                host,
                port,
                socketOption,
                cancellationToken);

            await smtp.AuthenticateAsync(
                username,
                password,
                cancellationToken);

            await smtp.SendAsync(
                message,
                cancellationToken);

            await smtp.DisconnectAsync(
                true,
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "SMTP email sending failed for {Email}",
                toEmail);

            throw new InvalidOperationException(
                "Unable to send email using the configured SMTP server.",
                ex);
        }
        finally
        {
            if (smtp.IsConnected)
            {
                try
                {
                    await smtp.DisconnectAsync(
                        true,
                        CancellationToken.None);
                }
                catch
                {
                    // Ignore disconnect errors
                }
            }
        }
    }
}