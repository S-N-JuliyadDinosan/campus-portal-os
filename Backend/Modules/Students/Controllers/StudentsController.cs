using CampusServicesPortal.Common.Pagination;
using CampusServicesPortal.Common.Responses;
using CampusServicesPortal.Common.Security;
using CampusServicesPortal.Modules.Students.DTOs;
using CampusServicesPortal.Modules.Students.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Students.Controllers;

[ApiController]
public sealed class StudentsController(
    IStudentService studentService,
    ICurrentUserService currentUser) : ControllerBase
{
    // API-010
    [AllowAnonymous]
    [HttpPost("api/students/register")]
    public async Task<ActionResult<ApiResponse<StudentRegistrationResponse>>> Register(
        [FromBody] StudentRegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await studentService.RegisterAsync(request, cancellationToken);
        return Created($"/api/students/{result.StudentId}",
            ApiResponse<StudentRegistrationResponse>.Ok(result, "Registration successful."));
    }

    // API-011
    [Authorize(Roles = "Student")]
    [HttpGet("api/students/me")]
    public async Task<ActionResult<ApiResponse<StudentProfileResponse>>> GetMe(
        CancellationToken cancellationToken)
    {
        var result = await studentService.GetMyProfileAsync(RequireStudentId(), cancellationToken);
        return Ok(ApiResponse<StudentProfileResponse>.Ok(result));
    }

    // API-012
    [Authorize(Roles = "Student")]
    [HttpPut("api/students/me")]
    public async Task<ActionResult<ApiResponse<StudentProfileResponse>>> UpdateMe(
        [FromBody] UpdateMyStudentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await studentService.UpdateMyProfileAsync(RequireStudentId(), request, cancellationToken);
        return Ok(ApiResponse<StudentProfileResponse>.Ok(result, "Profile updated."));
    }

    // API-013
    [Authorize(Roles = "Student")]
    [HttpGet("api/students/me/activity-summary")]
    public async Task<ActionResult<ApiResponse<StudentActivitySummaryResponse>>> ActivitySummary(
        CancellationToken cancellationToken)
    {
        var result = await studentService.GetActivitySummaryAsync(RequireStudentId(), cancellationToken);
        return Ok(ApiResponse<StudentActivitySummaryResponse>.Ok(result));
    }

    // API-014
    [Authorize(Roles = "Student,Admin")]
    [HttpGet("api/students/{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        if (User.IsInRole("Student"))
        {
            if (RequireStudentId() != id)
                return Forbid();

            var profile = await studentService.GetMyProfileAsync(id, cancellationToken);
            return Ok(ApiResponse<StudentProfileResponse>.Ok(profile));
        }

        var result = await studentService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<AdminStudentDetailResponse>.Ok(result));
    }

    // BRD endpoint: student updates own profile by id. IndexNumber is intentionally not present in the DTO.
    [Authorize(Roles = "Student")]
    [HttpPut("api/students/{id:int}")]
    public async Task<IActionResult> UpdateById(
        int id,
        [FromBody] UpdateMyStudentRequest request,
        CancellationToken cancellationToken)
    {
        if (RequireStudentId() != id)
            return Forbid();

        var result = await studentService.UpdateMyProfileAsync(id, request, cancellationToken);
        return Ok(ApiResponse<StudentProfileResponse>.Ok(result, "Profile updated."));
    }

    // API-015
    [Authorize(Roles = "Admin")]
    [HttpGet("api/students")]
    public async Task<ActionResult<ApiResponse<PagedResult<StudentListItemResponse>>>> Search(
        [FromQuery] string? search,
        [FromQuery] int? facultyId,
        [FromQuery] string? faculty,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await studentService.SearchAsync(search, facultyId, faculty, isActive, page, cancellationToken);
        return Ok(ApiResponse<PagedResult<StudentListItemResponse>>.Ok(result));
    }

    // API-016
    [Authorize(Roles = "Admin")]
    [HttpPut("api/admin/students/{id:int}")]
    public async Task<ActionResult<ApiResponse<StudentProfileResponse>>> AdminUpdate(
        int id,
        [FromBody] AdminUpdateStudentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await studentService.AdminUpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<StudentProfileResponse>.Ok(result, "Student updated."));
    }


    // BRD endpoint: deactivate a student account.
    [Authorize(Roles = "Admin")]
    [HttpDelete("api/students/{id:int}")]
    public async Task<IActionResult> DeleteStudent(
        int id,
        CancellationToken cancellationToken)
    {
        await studentService.DeactivateAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok("Student deactivated successfully."));
    }

    // API-017
    [Authorize(Roles = "Admin")]
    [HttpGet("api/admin/students/{id:int}/deactivation-check")]
    public async Task<ActionResult<ApiResponse<DeactivationCheckResponse>>> DeactivationCheck(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await studentService.CheckDeactivationAsync(id, cancellationToken);
        return Ok(ApiResponse<DeactivationCheckResponse>.Ok(result));
    }

    // API-018
    [Authorize(Roles = "Admin")]
    [HttpPut("api/admin/students/{id:int}/deactivate")]
    public async Task<ActionResult<ApiResponse<object>>> Deactivate(
        int id,
        CancellationToken cancellationToken)
    {
        await studentService.DeactivateAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok("Student deactivated successfully."));
    }

    // API-019
    [Authorize(Roles = "Admin")]
    [HttpPut("api/admin/students/{id:int}/reactivate")]
    public async Task<ActionResult<ApiResponse<object>>> Reactivate(
        int id,
        CancellationToken cancellationToken)
    {
        await studentService.ReactivateAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok("Student reactivated successfully."));
    }

    private int RequireStudentId() =>
        currentUser.StudentId ?? throw new UnauthorizedAccessException("Student id is missing from the access token.");
}
