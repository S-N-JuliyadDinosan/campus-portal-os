using System.Security.Cryptography;
using CampusServicesPortal.Common.Exceptions;
using CampusServicesPortal.Common.Security;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Identity.DTOs;
using CampusServicesPortal.Modules.Identity.Entities;
using CampusServicesPortal.Modules.Identity.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CampusServicesPortal.Modules.Identity.Services;

public sealed class AuthService(
    ApplicationDbContext dbContext,
    ITokenService tokenService,
    IPasswordHasher<User> passwordHasher,
    IEmailService emailService,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly JwtOptions _jwt = jwtOptions.Value;


    // =========================================================
    // LOGIN
    // =========================================================
    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);

        var user = await dbContext.Users
            .Include(x => x.Student)
            .SingleOrDefaultAsync(
                x => x.Email == email,
                cancellationToken);

        if (user is null)
        {
            throw new AuthenticationException(
                "Invalid email or password.");
        }

        var verification =
            passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

        if (verification ==
            PasswordVerificationResult.Failed)
        {
            throw new AuthenticationException(
                "Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new AuthenticationException(
                "This account is inactive.");
        }

        if (!user.EmailVerified)
        {
            throw new AuthenticationException(
                "Please verify your email before logging in.");
        }

        if (verification ==
            PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash =
                passwordHasher.HashPassword(
                    user,
                    request.Password);

            await dbContext.SaveChangesAsync(
                cancellationToken);
        }

        return await IssueSessionAsync(
            user,
            cancellationToken);
    }


    // =========================================================
    // REFRESH TOKEN
    // =========================================================
    public async Task<LoginResponse> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var hash =
            tokenService.HashToken(
                request.RefreshToken);

        var token = await dbContext.RefreshTokens
            .Include(x => x.User)
                .ThenInclude(x => x.Student)
            .SingleOrDefaultAsync(
                x => x.TokenHash == hash,
                cancellationToken);

        if (token is null ||
            token.RevokedAt is not null ||
            token.ExpiresAt <= DateTime.UtcNow)
        {
            throw new AuthenticationException(
                "Invalid or expired refresh token.");
        }

        if (!token.User.IsActive)
        {
            throw new AuthenticationException(
                "This account is inactive.");
        }

        if (!token.User.EmailVerified)
        {
            throw new AuthenticationException(
                "Please verify your email before refreshing the session.");
        }

        token.RevokedAt =
            DateTime.UtcNow;

        return await IssueSessionAsync(
            token.User,
            cancellationToken);
    }


    // =========================================================
    // LOGOUT
    // =========================================================
    public async Task LogoutAsync(
        int userId,
        LogoutRequest request,
        CancellationToken cancellationToken = default)
    {
        var hash =
            tokenService.HashToken(
                request.RefreshToken);

        var token =
            await dbContext.RefreshTokens
                .SingleOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.TokenHash == hash,
                    cancellationToken);

        if (token is not null &&
            token.RevokedAt is null)
        {
            token.RevokedAt =
                DateTime.UtcNow;

            await dbContext.SaveChangesAsync(
                cancellationToken);
        }
    }


    // =========================================================
    // LOGOUT ALL
    // =========================================================
    public async Task LogoutAllAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var now =
            DateTime.UtcNow;

        var tokens =
            await dbContext.RefreshTokens
                .Where(
                    x =>
                        x.UserId == userId &&
                        x.RevokedAt == null &&
                        x.ExpiresAt > now)
                .ToListAsync(
                    cancellationToken);

        foreach (var token in tokens)
        {
            token.RevokedAt =
                now;
        }

        var user =
            await dbContext.Users.FindAsync(
                [userId],
                cancellationToken);

        if (user is not null)
        {
            user.SecurityStamp =
                Guid.NewGuid().ToString("N");
        }

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }


    // =========================================================
    // CHANGE PASSWORD
    // =========================================================
    public async Task ChangePasswordAsync(
        int userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CurrentPassword ==
            request.NewPassword)
        {
            throw new BusinessRuleException(
                "New password must be different from the current password.");
        }

        var user =
            await dbContext.Users.FindAsync(
                [userId],
                cancellationToken)
            ?? throw new NotFoundException(
                "User not found.");

        var verified =
            passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.CurrentPassword);

        if (verified ==
            PasswordVerificationResult.Failed)
        {
            throw new AuthenticationException(
                "Current password is incorrect.");
        }

        user.PasswordHash =
            passwordHasher.HashPassword(
                user,
                request.NewPassword);

        user.SecurityStamp =
            Guid.NewGuid().ToString("N");

        await RevokeAllRefreshTokensAsync(
            userId,
            cancellationToken);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }


    // =========================================================
    // VERIFY EMAIL
    // EMAIL + 6 DIGIT OTP
    // =========================================================
    public async Task VerifyEmailAsync(
        VerifyEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        var email =
            NormalizeEmail(request.Email);

        var otp =
            request.Token.Trim();

        var hash =
            tokenService.HashToken(
                otp);

        var token =
            await dbContext.EmailVerificationTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(
                    x =>
                        x.User.Email == email &&
                        x.TokenHash == hash &&
                        x.UsedAt == null,
                    cancellationToken);

        if (token is null)
        {
            throw new BusinessRuleException(
                "Invalid verification OTP.");
        }

        if (!token.User.IsActive)
        {
            throw new BusinessRuleException(
                "This account is inactive.");
        }

        if (token.ExpiresAt <=
            DateTime.UtcNow)
        {
            throw new BusinessRuleException(
                "Verification OTP has expired.");
        }

        if (token.User.EmailVerified)
        {
            throw new BusinessRuleException(
                "Email has already been verified.");
        }

        var now =
            DateTime.UtcNow;

        var activeTokens =
            await dbContext.EmailVerificationTokens
                .Where(
                    x =>
                        x.UserId == token.UserId &&
                        x.UsedAt == null)
                .ToListAsync(
                    cancellationToken);

        foreach (var activeToken in activeTokens)
        {
            activeToken.UsedAt =
                now;
        }

        token.User.EmailVerified =
            true;

        token.User.SecurityStamp =
            Guid.NewGuid().ToString("N");

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }


    // =========================================================
    // RESEND EMAIL OTP
    // =========================================================
    public async Task ResendVerificationAsync(
        ResendVerificationRequest request,
        CancellationToken cancellationToken = default)
    {
        var email =
            NormalizeEmail(request.Email);

        var user =
            await dbContext.Users
                .SingleOrDefaultAsync(
                    x => x.Email == email,
                    cancellationToken);

        if (user is null ||
            user.EmailVerified ||
            !user.IsActive)
        {
            return;
        }

        var now =
            DateTime.UtcNow;

        var oldTokens =
            await dbContext.EmailVerificationTokens
                .Where(
                    x =>
                        x.UserId == user.UserId &&
                        x.UsedAt == null)
                .ToListAsync(
                    cancellationToken);

        foreach (var oldToken in oldTokens)
        {
            oldToken.UsedAt =
                now;
        }

        var rawOtp =
            tokenService.CreateOneTimeToken();

        dbContext.EmailVerificationTokens.Add(
            new EmailVerificationToken
            {
                UserId =
                    user.UserId,

                TokenHash =
                    tokenService.HashToken(
                        rawOtp),

                ExpiresAt =
                    now.AddHours(24)
            });

        await dbContext.SaveChangesAsync(
            cancellationToken);

        await emailService
            .SendEmailVerificationAsync(
                email,
                rawOtp,
                cancellationToken);
    }


    // =========================================================
    // FORGOT PASSWORD
    // =========================================================
    public async Task ForgotPasswordAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var email =
            NormalizeEmail(request.Email);

        var user =
            await dbContext.Users
                .SingleOrDefaultAsync(
                    x => x.Email == email,
                    cancellationToken);

        // Prevent account enumeration
        if (user is null ||
            !user.IsActive)
        {
            return;
        }

        var now =
            DateTime.UtcNow;

        var oldTokens =
            await dbContext.PasswordResetTokens
                .Where(
                    x =>
                        x.UserId == user.UserId &&
                        x.UsedAt == null)
                .ToListAsync(
                    cancellationToken);

        foreach (var oldToken in oldTokens)
        {
            oldToken.UsedAt =
                now;
        }

        // Password reset = strong long token
        // NOT 6 digit OTP
        var rawResetToken =
            CreateSecurePasswordResetToken();

        dbContext.PasswordResetTokens.Add(
            new PasswordResetToken
            {
                UserId =
                    user.UserId,

                TokenHash =
                    tokenService.HashToken(
                        rawResetToken),

                ExpiresAt =
                    now.AddHours(1)
            });

        await dbContext.SaveChangesAsync(
            cancellationToken);

        await emailService
            .SendPasswordResetAsync(
                email,
                rawResetToken,
                cancellationToken);
    }


    // =========================================================
    // RESET PASSWORD
    // =========================================================
    public async Task ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var rawToken =
            request.Token.Trim();

        var hash =
            tokenService.HashToken(
                rawToken);

        var token =
            await dbContext.PasswordResetTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(
                    x =>
                        x.TokenHash == hash &&
                        x.UsedAt == null,
                    cancellationToken);

        if (token is null)
        {
            throw new BusinessRuleException(
                "Invalid password reset token.");
        }

        if (token.ExpiresAt <=
            DateTime.UtcNow)
        {
            throw new BusinessRuleException(
                "Password reset token has expired.");
        }

        if (!token.User.IsActive)
        {
            throw new BusinessRuleException(
                "This account is inactive.");
        }


        // -----------------------------------------------------
        // CREATE NEW PASSWORD HASH
        // -----------------------------------------------------
        var newPasswordHash =
            passwordHasher.HashPassword(
                token.User,
                request.NewPassword);


        // -----------------------------------------------------
        // SANITY CHECK
        //
        // Verify the generated hash immediately before saving.
        // -----------------------------------------------------
        var hashCheck =
            passwordHasher.VerifyHashedPassword(
                token.User,
                newPasswordHash,
                request.NewPassword);

        if (hashCheck ==
            PasswordVerificationResult.Failed)
        {
            throw new InvalidOperationException(
                "Unable to create a valid password hash.");
        }


        token.User.PasswordHash =
            newPasswordHash;

        token.User.SecurityStamp =
            Guid.NewGuid().ToString("N");

        var now =
            DateTime.UtcNow;

        token.UsedAt =
            now;


        // Revoke existing login sessions
        await RevokeAllRefreshTokensAsync(
            token.UserId,
            cancellationToken);


        // Invalidate any other password reset tokens
        var otherTokens =
            await dbContext.PasswordResetTokens
                .Where(
                    x =>
                        x.UserId == token.UserId &&
                        x.UsedAt == null)
                .ToListAsync(
                    cancellationToken);

        foreach (var otherToken in otherTokens)
        {
            otherToken.UsedAt =
                now;
        }


        await dbContext.SaveChangesAsync(
            cancellationToken);
    }


    // =========================================================
    // ISSUE LOGIN SESSION
    // =========================================================
    private async Task<LoginResponse> IssueSessionAsync(
        User user,
        CancellationToken cancellationToken)
    {
        var accessToken =
            tokenService.CreateAccessToken(
                user,
                user.Student?.StudentId);

        var rawRefreshToken =
            tokenService.CreateRefreshToken();

        dbContext.RefreshTokens.Add(
            new RefreshToken
            {
                UserId =
                    user.UserId,

                TokenHash =
                    tokenService.HashToken(
                        rawRefreshToken),

                ExpiresAt =
                    DateTime.UtcNow.AddDays(
                        _jwt.RefreshTokenDays)
            });

        await dbContext.SaveChangesAsync(
            cancellationToken);

        return new LoginResponse(
            accessToken,
            rawRefreshToken,
            DateTime.UtcNow.AddMinutes(
                _jwt.AccessTokenMinutes));
    }


    // =========================================================
    // REVOKE ALL REFRESH TOKENS
    // =========================================================
    private async Task RevokeAllRefreshTokensAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        var now =
            DateTime.UtcNow;

        var tokens =
            await dbContext.RefreshTokens
                .Where(
                    x =>
                        x.UserId == userId &&
                        x.RevokedAt == null)
                .ToListAsync(
                    cancellationToken);

        foreach (var token in tokens)
        {
            token.RevokedAt =
                now;
        }
    }


    // =========================================================
    // CREATE STRONG PASSWORD RESET TOKEN
    // =========================================================
    private static string CreateSecurePasswordResetToken()
    {
        return Convert.ToHexString(
            RandomNumberGenerator.GetBytes(32));
    }


    // =========================================================
    // NORMALIZE EMAIL
    // =========================================================
    private static string NormalizeEmail(
        string email)
    {
        return email
            .Trim()
            .ToLowerInvariant();
    }
}