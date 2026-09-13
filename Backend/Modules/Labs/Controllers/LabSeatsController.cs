using CampusServicesPortal.Modules.Labs.DTOs;
using CampusServicesPortal.Modules.Labs.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Labs.Controllers;

[ApiController]
[Route("api")]
public sealed class LabSeatsController : ControllerBase
{
    private readonly ILabSeatService _seatService;

    public LabSeatsController(
        ILabSeatService seatService)
    {
        _seatService = seatService;
    }

    // =====================================================
    // GET ALL SEATS FOR A LAB
    // =====================================================
    [HttpGet("labs/{labId:int}/seats")]
    public async Task<ActionResult<List<LabSeatResponse>>> GetByLab(
        int labId)
    {
        try
        {
            var seats =
                await _seatService.GetByLabIdAsync(labId);

            return Ok(seats);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    // =====================================================
    // GET AVAILABLE SEATS
    //
    // Example:
    // /api/labs/1/seats/availability
    // ?date=2026-08-11&timeSlotId=4
    // =====================================================
    [HttpGet("labs/{labId:int}/seats/availability")]
    public async Task<ActionResult<List<LabSeatResponse>>>
        GetAvailability(
            int labId,
            [FromQuery] DateOnly date,
            [FromQuery] int timeSlotId)
    {
        try
        {
            var seats =
                await _seatService.GetAvailableSeatsAsync(
                    labId,
                    timeSlotId,
                    date);

            return Ok(seats);
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
    // CREATE LAB SEAT
    // ADMIN ONLY
    // =====================================================
    [Authorize(Roles = "Admin")]
    [HttpPost("labs/{labId:int}/seats")]
    public async Task<ActionResult<LabSeatResponse>> Create(
        int labId,
        [FromBody] CreateLabSeatRequest request)
    {
        try
        {
            var seat =
                await _seatService.CreateAsync(
                    labId,
                    request);

            return Created(
                $"/api/labs/{labId}/seats/{seat.LabSeatId}",
                seat);
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
    // UPDATE LAB SEAT
    // ADMIN ONLY
    // =====================================================
    [Authorize(Roles = "Admin")]
    [HttpPut("labs/{labId:int}/seats/{seatId:int}")]
    public async Task<IActionResult> Update(
        int labId,
        int seatId,
        [FromBody] UpdateLabSeatRequest request)
    {
        try
        {
            var seat =
                await _seatService.GetByIdAsync(seatId);

            if (seat == null ||
                seat.LabId != labId)
            {
                return NotFound(new
                {
                    message = "Lab seat not found."
                });
            }

            var updated =
                await _seatService.UpdateAsync(
                    seatId,
                    request);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Lab seat not found."
                });
            }

            return NoContent();
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
    // DELETE / DEACTIVATE LAB SEAT
    // ADMIN ONLY
    // =====================================================
    [Authorize(Roles = "Admin")]
    [HttpDelete("labs/{labId:int}/seats/{seatId:int}")]
    public async Task<IActionResult> Delete(
        int labId,
        int seatId)
    {
        try
        {
            var seat =
                await _seatService.GetByIdAsync(seatId);

            if (seat == null ||
                seat.LabId != labId)
            {
                return NotFound(new
                {
                    message = "Lab seat not found."
                });
            }

            var deleted =
                await _seatService.DeleteAsync(
                    seatId);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Lab seat not found."
                });
            }

            return NoContent();
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