using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Modules.Identity.DTOs;


// ============================================================
// LOGIN
// ============================================================

public sealed record LoginRequest(
    [Required, EmailAddress]
    string Email,

    [Required]
    string Password);


// ============================================================
// LOGIN RESPONSE
// ============================================================

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt);


// ============================================================
// REFRESH TOKEN
// ============================================================

public sealed record RefreshTokenRequest(
    [Required]
    string RefreshToken);


// ============================================================
// LOGOUT
// ============================================================

public sealed record LogoutRequest(
    [Required]
    string RefreshToken);


// ============================================================
// CHANGE PASSWORD
// ============================================================

public sealed record ChangePasswordRequest(
    [Required]
    string CurrentPassword,

    [Required, MinLength(8)]
    string NewPassword);


// ============================================================
// EMAIL VERIFICATION - EMAIL + 6 DIGIT OTP
// ============================================================

public sealed record VerifyEmailRequest(

    [Required, EmailAddress]
    string Email,

    [Required]
    [RegularExpression(
        @"^\d{6}$",
        ErrorMessage = "OTP must contain exactly 6 digits.")]
    string Token
);


// ============================================================
// RESEND EMAIL VERIFICATION OTP
// ============================================================

public sealed record ResendVerificationRequest(
    [Required, EmailAddress]
    string Email);


// ============================================================
// FORGOT PASSWORD
// ============================================================

public sealed record ForgotPasswordRequest(
    [Required, EmailAddress]
    string Email);


// ============================================================
// RESET PASSWORD
// ============================================================

public sealed record ResetPasswordRequest(

    [Required]
    string Token,

    [Required, MinLength(8)]
    string NewPassword
);