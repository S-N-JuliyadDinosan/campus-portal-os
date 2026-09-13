using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Modules.Students.DTOs;

public sealed record CreateAdminRequest(
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Required, MinLength(8)] string Password);

public sealed record UpdateAdminRequest(
    [Required, EmailAddress, MaxLength(256)] string Email);

public sealed record AdminAccountResponse(
    int UserId,
    string Email,
    bool IsActive,
    bool EmailVerified,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
