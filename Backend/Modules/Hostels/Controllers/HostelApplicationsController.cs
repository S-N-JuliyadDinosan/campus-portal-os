using CampusServicesPortal.Common.Security;
using CampusServicesPortal.Modules.Hostels.DTOs;
using CampusServicesPortal.Modules.Hostels.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Hostels.Controllers;

[ApiController]
[Authorize]
[Route("api/hostel-applications")]
public sealed class HostelApplicationsController : ControllerBase
{
    private readonly IHostelApplicationService _applicationService;
    private readonly ICurrentUserService _currentUserService;

    public HostelApplicationsController(
        IHostelApplicationService applicationService,
        ICurrentUserService currentUserService)
    {
        _applicationService = applicationService;
        _currentUserService = currentUserService;
    }


    // =========================================================
    // STUDENT - CREATE HOSTEL APPLICATION
    // POST: /api/hostel-applications
    // =========================================================
    [Authorize(Roles = "Student")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateHostelApplicationRequest request)
    {
        try
        {
            var application =
                await _applicationService
                    .CreateAsync(request);

            return StatusCode(
                StatusCodes.Status201Created,
                application);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }


    // =========================================================
    // STUDENT - GET MY APPLICATIONS
    // GET: /api/hostel-applications/me
    // =========================================================
    [Authorize(Roles = "Student")]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyApplications()
    {
        if (!_currentUserService.StudentId.HasValue)
        {
            return Unauthorized(new
            {
                message =
                    "Student identity was not found in the access token."
            });
        }

        var studentId =
            _currentUserService.StudentId.Value;

        var applications =
            await _applicationService
                .GetByStudentIdAsync(
                    studentId);

        return Ok(applications);
    }


    // =========================================================
    // BRD - GET APPLICATIONS FOR A STUDENT
    // GET: /api/hostel-applications/student/{studentId}
    // =========================================================
    [Authorize(Roles = "Student,Admin")]
    [HttpGet("student/{studentId:int}")]
    public async Task<IActionResult> GetStudentApplications(int studentId)
    {
        if (User.IsInRole("Student"))
        {
            if (!_currentUserService.StudentId.HasValue ||
                _currentUserService.StudentId.Value != studentId)
            {
                return Forbid();
            }
        }

        var applications =
            await _applicationService.GetByStudentIdAsync(studentId);

        return Ok(applications);
    }


    // =========================================================
    // STUDENT / ADMIN - GET APPLICATION BY ID
    // GET: /api/hostel-applications/{id}
    // =========================================================
    [Authorize(Roles = "Student,Admin")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id)
    {
        var application =
            await _applicationService
                .GetByIdAsync(id);

        if (application == null)
        {
            return NotFound(new
            {
                message =
                    "Hostel application not found."
            });
        }


        // Student can only view their own application.
        if (string.Equals(
                _currentUserService.Role,
                "Student",
                StringComparison.OrdinalIgnoreCase))
        {
            if (!_currentUserService.StudentId.HasValue)
            {
                return Unauthorized(new
                {
                    message =
                        "Student identity was not found."
                });
            }

            if (application.StudentId !=
                _currentUserService.StudentId.Value)
            {
                return Forbid();
            }
        }


        return Ok(application);
    }


    // =========================================================
    // STUDENT - CANCEL OWN PENDING APPLICATION
    // DELETE: /api/hostel-applications/{id}
    // =========================================================
    [Authorize(Roles = "Student")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancel(
        int id)
    {
        try
        {
            var cancelled =
                await _applicationService
                    .CancelAsync(id);

            if (!cancelled)
            {
                return NotFound(new
                {
                    message =
                        "Hostel application not found."
                });
            }

            return Ok(new
            {
                message =
                    "Hostel application cancelled successfully."
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    message = ex.Message
                });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }


    // =========================================================
    // ADMIN - GET ALL APPLICATIONS
    //
    // Example:
    // GET /api/hostel-applications
    // ?status=Pending
    // &hostelId=1
    // &academicYear=2026/2027
    // &semester=1
    // &page=1
    // =========================================================
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] int? hostelId,
        [FromQuery] string? academicYear,
        [FromQuery] string? semester,
        [FromQuery] int page = 1)
    {
        try
        {
            var applications =
                await _applicationService
                    .GetAllAsync(
                        hostelId,
                        status,
                        academicYear,
                        semester,
                        page);

            return Ok(applications);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    // =========================================================
    // ADMIN - APPROVE / REJECT APPLICATION
    // PUT: /api/hostel-applications/{id}/status
    // =========================================================
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody]
        UpdateHostelApplicationStatusRequest request)
    {
        try
        {
            var adminUserId =
                GetCurrentUserId();

            var updated =
                await _applicationService
                    .UpdateStatusAsync(
                        id,
                        request,
                        adminUserId);

            if (!updated)
            {
                return NotFound(new
                {
                    message =
                        "Hostel application not found."
                });
            }

            return Ok(new
            {
                message =
                    "Application status updated successfully."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }


    // =========================================================
    // ADMIN - ASSIGN ROOM
    // PUT: /api/hostel-applications/{id}/assign-room
    // =========================================================
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/assign-room")]
    public async Task<IActionResult> AssignRoom(
        int id,
        [FromBody] AssignRoomRequest request)
    {
        try
        {
            var adminUserId =
                GetCurrentUserId();

            var assigned =
                await _applicationService
                    .AssignRoomAsync(
                        id,
                        request,
                        adminUserId);

            if (!assigned)
            {
                return NotFound(new
                {
                    message =
                        "Hostel application not found."
                });
            }

            return Ok(new
            {
                message =
                    "Room assigned successfully."
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }


    // =========================================================
    // ADMIN - UNASSIGN ROOM
    // PUT: /api/hostel-applications/{id}/unassign-room
    // =========================================================
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/unassign-room")]
    public async Task<IActionResult> UnassignRoom(
        int id)
    {
        try
        {
            var adminUserId =
                GetCurrentUserId();

            var unassigned =
                await _applicationService
                    .UnassignRoomAsync(
                        id,
                        adminUserId);

            if (!unassigned)
            {
                return NotFound(new
                {
                    message =
                        "Hostel application not found."
                });
            }

            return Ok(new
            {
                message =
                    "Room unassigned successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }


    // =========================================================
    // CURRENT LOGGED-IN USER ID
    // =========================================================
    private int GetCurrentUserId()
    {
        if (!_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "Current user identity was not found.");
        }

        return _currentUserService.UserId.Value;
    }
}