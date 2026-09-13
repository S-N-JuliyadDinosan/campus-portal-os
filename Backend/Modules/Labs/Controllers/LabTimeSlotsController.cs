using CampusServicesPortal.Modules.Labs.DTOs;
using CampusServicesPortal.Modules.Labs.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Labs.Controllers;

[ApiController]
[Route("api")]
public sealed class LabTimeSlotsController : ControllerBase
{
    private readonly ILabTimeSlotService _timeSlotService;

    public LabTimeSlotsController(
        ILabTimeSlotService timeSlotService)
    {
        _timeSlotService = timeSlotService;
    }


    // =====================================================
    // GET TIME SLOT BY ID
    // =====================================================
    [HttpGet("lab-time-slots/{id:int}")]
    public async Task<ActionResult<LabTimeSlotResponse>> GetById(
        int id)
    {
        var timeSlot =
            await _timeSlotService.GetByIdAsync(id);

        if (timeSlot == null)
        {
            return NotFound(new
            {
                message = "Lab time slot not found."
            });
        }

        return Ok(timeSlot);
    }


    // =====================================================
    // CREATE TIME SLOT
    // ADMIN ONLY
    // =====================================================
    [Authorize(Roles = "Admin")]
    [HttpPost("labs/{labId:int}/time-slots")]
    public async Task<ActionResult<LabTimeSlotResponse>> Create(
        int labId,
        [FromBody] CreateLabTimeSlotRequest request)
    {
        try
        {
            var timeSlot =
                await _timeSlotService.CreateAsync(
                    labId,
                    request);

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    id = timeSlot.LabTimeSlotId
                },
                timeSlot);
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
    // UPDATE TIME SLOT
    // ADMIN ONLY
    // =====================================================
    [Authorize(Roles = "Admin")]
    [HttpPut("lab-time-slots/{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateLabTimeSlotRequest request)
    {
        try
        {
            var updated =
                await _timeSlotService.UpdateAsync(
                    id,
                    request);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Lab time slot not found."
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
    // DELETE / DEACTIVATE TIME SLOT
    // ADMIN ONLY
    // =====================================================
    [Authorize(Roles = "Admin")]
    [HttpDelete("lab-time-slots/{id:int}")]
    public async Task<IActionResult> Delete(
        int id)
    {
        try
        {
            var deleted =
                await _timeSlotService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Lab time slot not found."
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