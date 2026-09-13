using CampusServicesPortal.Modules.Hostels.DTOs;
using CampusServicesPortal.Modules.Hostels.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Hostels.Controllers;

[ApiController]
[Route("api/hostels")]
public sealed class HostelsController : ControllerBase
{
    private readonly IHostelService _hostelService;

    public HostelsController(IHostelService hostelService)
    {
        _hostelService = hostelService;
    }

    // GET: /api/hostels
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var hostels =
            await _hostelService.GetAllAsync();

        return Ok(hostels);
    }

    // GET: /api/hostels/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var hostel =
            await _hostelService.GetByIdAsync(id);

        if (hostel == null)
        {
            return NotFound(new
            {
                message = "Hostel not found."
            });
        }

        return Ok(hostel);
    }

    // POST: /api/hostels
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateHostelRequest request)
    {
        try
        {
            var hostel =
                await _hostelService.CreateAsync(request);

            return StatusCode(
                StatusCodes.Status201Created,
                hostel);
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

    // PUT: /api/hostels/1
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateHostelRequest request)
    {
        try
        {
            var updated =
                await _hostelService.UpdateAsync(
                    id,
                    request);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Hostel not found."
                });
            }

            return Ok(new
            {
                message = "Hostel updated successfully."
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

    // DELETE: /api/hostels/1
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted =
            await _hostelService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Hostel not found."
            });
        }

        return Ok(new
        {
            message = "Hostel permanently deleted successfully."
        });
    }

    // GET:
    // /api/hostels/1/availability
    // ?semester=1&academicYear=2026/2027
    [HttpGet("{id:int}/availability")]
    public async Task<IActionResult> GetAvailability(
        int id,
        [FromQuery] string semester,
        [FromQuery] string academicYear)
    {
        try
        {
            var availability =
                await _hostelService.GetAvailabilityAsync(
                    id,
                    academicYear,
                    semester);

            if (availability == null)
            {
                return NotFound(new
                {
                    message = "Hostel not found."
                });
            }

            return Ok(availability);
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
