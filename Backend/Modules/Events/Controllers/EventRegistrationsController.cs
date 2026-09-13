using CampusServicesPortal.Common.Security;
using CampusServicesPortal.Modules.Events.DTOs.EventRegistrations;
using CampusServicesPortal.Modules.Events.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Events.Controllers;

[ApiController]
[Authorize]
[Route("api/event-registrations")]
public sealed class EventRegistrationsController : ControllerBase
{
    private readonly IEventRegistrationService _registrationService;
    private readonly ICurrentUserService _currentUserService;

    public EventRegistrationsController(
        IEventRegistrationService registrationService,
        ICurrentUserService currentUserService)
    {
        _registrationService = registrationService;
        _currentUserService = currentUserService;
    }


    // =====================================================
    // STUDENT - REGISTER / CREATE HOLD
    //
    // StudentId comes ONLY from JWT.
    // Never trust studentId from query/body.
    // =====================================================
    [Authorize(Roles = "Student")]
    [HttpPost]
    public async Task<ActionResult<EventRegistrationResponseDto>> Register(
        [FromBody] CreateEventRegistrationDto dto)
    {
        try
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

            var registration =
                await _registrationService.RegisterAsync(
                    studentId,
                    dto);

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    registrationId =
                        registration.EventRegistrationId
                },
                registration);
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


    // =====================================================
    // STUDENT - CONFIRM OWN HELD REGISTRATION
    // =====================================================
    [Authorize(Roles = "Student")]
    [HttpPut("{registrationId:int}/confirm")]
    public async Task<IActionResult> Confirm(
        int registrationId)
    {
        try
        {
            if (!_currentUserService.StudentId.HasValue)
            {
                return Unauthorized(new
                {
                    message =
                        "Student identity was not found in the access token."
                });
            }

            var registration =
                await _registrationService
                    .GetByIdAsync(registrationId);

            if (registration == null)
            {
                return NotFound(new
                {
                    message = "Event registration not found."
                });
            }

            if (registration.StudentId !=
                _currentUserService.StudentId.Value)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        message =
                            "You cannot confirm another student's event registration."
                    });
            }

            await _registrationService
                .ConfirmAsync(registrationId);

            return Ok(new
            {
                message =
                    "Event registration confirmed successfully."
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


    // =====================================================
    // STUDENT - GET OWN REGISTRATIONS
    //
    // No studentId in URL.
    // StudentId is taken from JWT.
    // =====================================================
    [Authorize(Roles = "Student")]
    [HttpGet("me")]
    public async Task<
        ActionResult<IEnumerable<MyEventRegistrationDto>>>
        GetMine()
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

        var registrations =
            await _registrationService
                .GetMyRegistrationsAsync(
                    studentId);

        return Ok(registrations);
    }


    // =====================================================
    // BRD - GET REGISTRATIONS FOR A STUDENT
    // GET /api/event-registrations/student/{studentId}
    // =====================================================
    [Authorize(Roles = "Student,Admin")]
    [HttpGet("student/{studentId:int}")]
    public async Task<IActionResult> GetStudentRegistrations(int studentId)
    {
        if (User.IsInRole("Student"))
        {
            if (!_currentUserService.StudentId.HasValue ||
                _currentUserService.StudentId.Value != studentId)
            {
                return Forbid();
            }

            var mine = await _registrationService.GetMyRegistrationsAsync(studentId);
            return Ok(mine);
        }

        var registrations = await _registrationService.GetAllAsync(
            new RegistrationFilterDto { StudentId = studentId, Page = 1, PageSize = 100 });
        return Ok(registrations);
    }


    // =====================================================
    // STUDENT / ADMIN - GET REGISTRATION BY ID
    //
    // Student can only view own registration.
    // Admin can view any registration.
    // =====================================================
    [Authorize(Roles = "Student,Admin")]
    [HttpGet("{registrationId:int}")]
    public async Task<ActionResult<EventRegistrationResponseDto>>
        GetById(
            int registrationId)
    {
        var registration =
            await _registrationService
                .GetByIdAsync(registrationId);

        if (registration == null)
        {
            return NotFound(new
            {
                message = "Event registration not found."
            });
        }

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

            if (registration.StudentId !=
                _currentUserService.StudentId.Value)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        message =
                            "You cannot view another student's event registration."
                    });
            }
        }

        return Ok(registration);
    }


    // =====================================================
    // ADMIN - GET ALL EVENT REGISTRATIONS
    // =====================================================
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<
        ActionResult<IEnumerable<EventRegistrationResponseDto>>>
        GetAll(
            [FromQuery] RegistrationFilterDto filter)
    {
        try
        {
            var registrations =
                await _registrationService
                    .GetAllAsync(filter);

            return Ok(registrations);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    // =====================================================
    // STUDENT - CANCEL OWN REGISTRATION
    // =====================================================
    [Authorize(Roles = "Student")]
    [HttpDelete("{registrationId:int}")]
    public async Task<IActionResult> Cancel(
        int registrationId)
    {
        try
        {
            if (!_currentUserService.StudentId.HasValue)
            {
                return Unauthorized(new
                {
                    message =
                        "Student identity was not found in the access token."
                });
            }

            var registration =
                await _registrationService
                    .GetByIdAsync(registrationId);

            if (registration == null)
            {
                return NotFound(new
                {
                    message = "Event registration not found."
                });
            }

            if (registration.StudentId !=
                _currentUserService.StudentId.Value)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        message =
                            "You cannot cancel another student's event registration."
                    });
            }

            await _registrationService
                .CancelAsync(registrationId);

            return Ok(new
            {
                message =
                    "Event registration cancelled successfully."
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
}