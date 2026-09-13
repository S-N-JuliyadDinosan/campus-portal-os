using CampusServicesPortal.Modules.Labs.DTOs;
using CampusServicesPortal.Modules.Labs.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Labs.Controllers;

[ApiController]
[Route("api/labs")]
public sealed class LabsController : ControllerBase
{
    private readonly ILabService _labService;
    private readonly ILabTimeSlotService _timeSlotService;

    public LabsController(
        ILabService labService,
        ILabTimeSlotService timeSlotService)
    {
        _labService = labService;
        _timeSlotService = timeSlotService;
    }

    // =====================================================
    // GET ALL LABS
    // Student / Admin / Public read
    // =====================================================
    [HttpGet]
    public async Task<ActionResult<List<LabResponse>>> GetAll()
    {
        var labs =
            await _labService.GetAllAsync();

        return Ok(labs);
    }

    // =====================================================
    // GET LAB BY ID
    // =====================================================
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LabResponse>> GetById(
        int id)
    {
        var lab =
            await _labService.GetByIdAsync(id);

        if (lab == null)
        {
            return NotFound(new
            {
                message = "Lab not found."
            });
        }

        return Ok(lab);
    }

    // =====================================================
    // CREATE LAB
    // ADMIN ONLY
    // =====================================================
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<LabResponse>> Create(
        [FromBody] CreateLabRequest request)
    {
        try
        {
            var lab =
                await _labService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    id = lab.LabId
                },
                lab);
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
    // UPDATE LAB
    // ADMIN ONLY
    // =====================================================
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateLabRequest request)
    {
        try
        {
            var updated =
                await _labService.UpdateAsync(
                    id,
                    request);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Lab not found."
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
    // DELETE / DEACTIVATE LAB
    // ADMIN ONLY
    // =====================================================
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id)
    {
        try
        {
            var deleted =
                await _labService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Lab not found."
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

    // =====================================================
    // GET AVAILABLE SLOTS FOR DATE
    // =====================================================
    [HttpGet("{labId:int}/slots")]
    public async Task<
        ActionResult<List<LabTimeSlotResponse>>>
        GetAvailableSlots(
            int labId,
            [FromQuery] DateOnly date)
    {
        try
        {
            var slots =
                await _timeSlotService
                    .GetAvailableByDateAsync(
                        labId,
                        date);

            return Ok(slots);
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
}