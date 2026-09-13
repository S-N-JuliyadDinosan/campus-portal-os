using CampusServicesPortal.Common.Security;
using CampusServicesPortal.Modules.Dashboards.DTOs;
using CampusServicesPortal.Modules.Dashboards.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Dashboards.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public sealed class DashboardsController(
    IDashboardService dashboardService,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet("student")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<StudentDashboardDto>> GetStudent(
        CancellationToken cancellationToken)
    {
        var studentId = currentUser.StudentId
            ?? throw new UnauthorizedAccessException(
                "Student identity was not found in the access token.");

        return Ok(await dashboardService.GetStudentDashboardAsync(
            studentId, cancellationToken));
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminDashboardDto>> GetAdmin(
        CancellationToken cancellationToken)
    {
        return Ok(await dashboardService.GetAdminDashboardAsync(
            cancellationToken));
    }

    // Convenience route for the Angular app; returns the dashboard for the JWT role.
    [HttpGet]
    public async Task<IActionResult> GetCurrent(
        CancellationToken cancellationToken)
    {
        if (User.IsInRole("Admin"))
        {
            return Ok(await dashboardService.GetAdminDashboardAsync(
                cancellationToken));
        }

        if (User.IsInRole("Student"))
        {
            var studentId = currentUser.StudentId
                ?? throw new UnauthorizedAccessException(
                    "Student identity was not found in the access token.");

            return Ok(await dashboardService.GetStudentDashboardAsync(
                studentId, cancellationToken));
        }

        return Forbid();
    }
}
