using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Modules.Students.DTOs;

public sealed record StudentRegisterRequest(
    [Required, MaxLength(50)] string IndexNumber,
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Required, MinLength(8)] string Password,
    [MaxLength(30)] string? PhoneNumber,
    [MaxLength(500)] string? Address,
    [MaxLength(200)] string? FullName = null,
    int? FacultyId = null);

public sealed record StudentRegistrationResponse(
    int StudentId,
    int UserId,
    string IndexNumber,
    string FullName,
    string Email,
    bool EmailVerificationRequired);

public sealed record StudentProfileResponse(
    int StudentId,
    int UserId,
    string IndexNumber,
    string FullName,
    string Email,
    string? PhoneNumber,
    string? Address,
    int FacultyId,
    string FacultyCode,
    string FacultyName,
    bool IsActive,
    bool EmailVerified,
    DateTime? DeactivatedAt);

public sealed record UpdateMyStudentRequest(
    [MaxLength(30)] string? PhoneNumber,
    [MaxLength(500)] string? Address);

public sealed record AdminUpdateStudentRequest(
    [EmailAddress, MaxLength(256)] string? Email,
    [MaxLength(200)] string? FullName,
    int? FacultyId,
    [MaxLength(30)] string? PhoneNumber,
    [MaxLength(500)] string? Address);

public sealed record StudentListItemResponse(
    int StudentId,
    string IndexNumber,
    string FullName,
    string Email,
    int FacultyId,
    string FacultyCode,
    bool IsActive);

public sealed record StudentActivitySummaryResponse(
    int HostelApplications,
    int LabBookings,
    int EventRegistrations,
    int Complaints,
    int CertificateRequests,
    int OutstandingFees);

public sealed record AdminStudentDetailResponse(
    StudentProfileResponse Profile,
    StudentActivitySummaryResponse Activity);

public sealed record DeactivationCheckResponse(
    bool CanDeactivate,
    IReadOnlyCollection<string> BlockingCommitments);
