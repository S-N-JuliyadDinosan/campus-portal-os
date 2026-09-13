using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Modules.Students.DTOs;

public sealed record StudentMasterCheckResponse(
    string IndexNumber,
    bool Exists,
    bool IsActive,
    bool AlreadyRegistered);

public sealed record StudentMasterResponse(
    int StudentMasterId,
    string IndexNumber,
    string FullName,
    string? OfficialEmail,
    int FacultyId,
    string FacultyCode,
    string FacultyName,
    int IntakeYear,
    bool IsActive,
    bool AlreadyRegistered);

public sealed record CreateStudentMasterRequest(
    [Required, MaxLength(50)] string IndexNumber,
    [Required, MaxLength(200)] string FullName,
    [EmailAddress, MaxLength(256)] string? OfficialEmail,
    int FacultyId,
    int IntakeYear,
    bool IsActive = true);

public sealed record UpdateStudentMasterRequest(
    [Required, MaxLength(50)] string IndexNumber,
    [Required, MaxLength(200)] string FullName,
    [EmailAddress, MaxLength(256)] string? OfficialEmail,
    int FacultyId,
    int IntakeYear,
    bool IsActive);

public sealed record StudentMasterImportResponse(
    bool Success,
    int TotalRows,
    int Inserted,
    int Updated,
    IReadOnlyCollection<string> Errors);
