using CampusServicesPortal.Common.Security;
using CampusServicesPortal.Modules.Labs.DTOs;
using CampusServicesPortal.Modules.Labs.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Labs.Controllers;

[ApiController]
[Authorize]
[Route("api/lab-bookings")]
public sealed class LabBookingsController : ControllerBase
{
    private readonly ILabBookingService _bookingService;
    private readonly ICurrentUserService _currentUserService;

    public LabBookingsController(
        ILabBookingService bookingService,
        ICurrentUserService currentUserService)
    {
        _bookingService = bookingService;
        _currentUserService = currentUserService;
    }


    // =========================================================
    // STUDENT - CREATE LAB BOOKING
    // POST /api/lab-bookings
    // =========================================================
    [Authorize(Roles = "Student")]
    [HttpPost]
    public async Task<ActionResult<LabBookingResponse>> Create(
        [FromBody] CreateLabBookingRequest request)
    {
        try
        {
            var booking =
                await _bookingService
                    .CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    id = booking.LabBookingId
                },
                booking);
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
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    message = ex.Message
                });
        }
    }


    // =========================================================
    // STUDENT - GET MY BOOKINGS
    // GET /api/lab-bookings/me
    // =========================================================
    [Authorize(Roles = "Student")]
    [HttpGet("me")]
    public async Task<
        ActionResult<List<LabBookingResponse>>>
        GetMine()
    {
        if (!_currentUserService
            .StudentId
            .HasValue)
        {
            return Unauthorized(new
            {
                message =
                    "Student identity was not found in the access token."
            });
        }


        var studentId =
            _currentUserService
                .StudentId
                .Value;


        var bookings =
            await _bookingService
                .GetByStudentIdAsync(
                    studentId);


        return Ok(bookings);
    }


    // =========================================================
    // BRD - GET BOOKINGS FOR A STUDENT
    // GET /api/lab-bookings/student/{studentId}
    // =========================================================
    [Authorize(Roles = "Student,Admin")]
    [HttpGet("student/{studentId:int}")]
    public async Task<IActionResult> GetStudentBookings(int studentId)
    {
        if (User.IsInRole("Student"))
        {
            if (!_currentUserService.StudentId.HasValue ||
                _currentUserService.StudentId.Value != studentId)
            {
                return Forbid();
            }
        }

        var bookings = await _bookingService.GetByStudentIdAsync(studentId);
        return Ok(bookings);
    }


    // =========================================================
    // STUDENT / ADMIN - GET BOOKING BY ID
    // GET /api/lab-bookings/{id}
    // =========================================================
    [Authorize(Roles = "Student,Admin")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LabBookingResponse>> GetById(
        int id)
    {
        var booking =
            await _bookingService
                .GetByIdAsync(id);


        if (booking == null)
        {
            return NotFound(new
            {
                message =
                    "Lab booking not found."
            });
        }


        // Student must only see their own booking.
        if (string.Equals(
                _currentUserService.Role,
                "Student",
                StringComparison.OrdinalIgnoreCase))
        {
            if (!_currentUserService
                .StudentId
                .HasValue)
            {
                return Unauthorized(new
                {
                    message =
                        "Student identity was not found."
                });
            }


            if (booking.StudentId !=
                _currentUserService
                    .StudentId
                    .Value)
            {
                return Forbid();
            }
        }


        return Ok(booking);
    }


    // =========================================================
    // STUDENT - CONFIRM OWN HELD BOOKING
    // PUT /api/lab-bookings/{id}/confirm
    // =========================================================
    [Authorize(Roles = "Student")]
    [HttpPut("{id:int}/confirm")]
    public async Task<IActionResult> Confirm(
        int id)
    {
        try
        {
            var confirmed =
                await _bookingService
                    .ConfirmAsync(id);


            if (!confirmed)
            {
                return NotFound(new
                {
                    message =
                        "Lab booking not found."
                });
            }


            return Ok(new
            {
                message =
                    "Lab booking confirmed successfully."
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
    // STUDENT - CANCEL OWN BOOKING
    // DELETE /api/lab-bookings/{id}
    // =========================================================
    [Authorize(Roles = "Student")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancel(
        int id)
    {
        try
        {
            var cancelled =
                await _bookingService
                    .CancelAsync(id);


            if (!cancelled)
            {
                return NotFound(new
                {
                    message =
                        "Lab booking not found."
                });
            }


            return Ok(new
            {
                message =
                    "Lab booking cancelled successfully."
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
    // ADMIN - GET ALL LAB BOOKINGS
    //
    // GET /api/lab-bookings
    // ?status=Held
    // &labId=1
    // &date=2026-08-11
    // &studentId=3
    // &page=1
    // =========================================================
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<
        ActionResult<List<LabBookingResponse>>>
        GetAll(
            [FromQuery] string? status = null,
            [FromQuery] int? labId = null,
            [FromQuery] DateOnly? date = null,
            [FromQuery] int? studentId = null,
            [FromQuery] int page = 1)
    {
        try
        {
            var bookings =
                await _bookingService
                    .GetAllAsync(
                        labId,
                        studentId,
                        status,
                        date,
                        page);


            return Ok(bookings);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}