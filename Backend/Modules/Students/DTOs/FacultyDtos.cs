using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Modules.Students.DTOs;

public sealed record FacultyResponse(
    int FacultyId,
    string Code,
    string Name,
    bool IsActive);

public sealed record CreateFacultyRequest(
    [Required, MaxLength(30)] string Code,
    [Required, MaxLength(150)] string Name);

public sealed record UpdateFacultyRequest(
    [Required, MaxLength(30)] string Code,
    [Required, MaxLength(150)] string Name,
    bool IsActive);

public sealed record FacultyDeleteResponse(
    int FacultyId,
    string Action,
    string Message);
