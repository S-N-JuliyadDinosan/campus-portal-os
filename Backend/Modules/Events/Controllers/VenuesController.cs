using CampusService.Modules.Events.DTOs.Venues;
using CampusServicesPortal.Modules.Events.DTOs.Venues;
using CampusServicesPortal.Modules.Events.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Events.Controllers;

[ApiController]
[Route("api/venues")]
public sealed class VenuesController : ControllerBase
{
    private readonly IVenueService _venueService;

    public VenuesController(
        IVenueService venueService)
    {
        _venueService = venueService;
    }


    // =====================================================
    // GET ALL VENUES
    // PUBLIC
    // =====================================================
    [HttpGet]
    public async Task<
        ActionResult<IEnumerable<VenueResponseDto>>> GetAll()
    {
        var venues =
            await _venueService.GetAllAsync();

        return Ok(venues);
    }


    // =====================================================
    // GET VENUE BY ID
    // PUBLIC
    // =====================================================
    [HttpGet("{venueId:int}")]
    public async Task<ActionResult<VenueResponseDto>> GetById(
        int venueId)
    {
        var venue =
            await _venueService.GetByIdAsync(
                venueId);

        if (venue is null)
        {
            return NotFound(new
            {
                message =
                    $"Venue with ID {venueId} was not found."
            });
        }

        return Ok(venue);
    }


    // =====================================================
    // CHECK VENUE AVAILABILITY
    // PUBLIC
    // =====================================================
    // GET:
    // /api/venues/5/availability
    // ?from=2026-08-10T09:00:00
    // &to=2026-08-10T12:00:00
    [HttpGet("{venueId:int}/availability")]
    public async Task<
        ActionResult<VenueAvailabilityResponseDto>>
        CheckAvailability(
            int venueId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
    {
        if (from >= to)
        {
            return BadRequest(new
            {
                message =
                    "The 'from' time must be before the 'to' time."
            });
        }

        try
        {
            var availability =
                await _venueService
                    .CheckAvailabilityAsync(
                        venueId,
                        from,
                        to);

            return Ok(availability);
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
    // CREATE VENUE
    // ADMIN ONLY
    // =====================================================
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<VenueResponseDto>> Create(
        [FromBody] CreateVenueDto dto)
    {
        try
        {
            var venue =
                await _venueService
                    .CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    venueId =
                        venue.VenueId
                },
                venue);
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
    // UPDATE VENUE
    // ADMIN ONLY
    // =====================================================
    [Authorize(Roles = "Admin")]
    [HttpPut("{venueId:int}")]
    public async Task<IActionResult> Update(
        int venueId,
        [FromBody] UpdateVenueDto dto)
    {
        try
        {
            await _venueService
                .UpdateAsync(
                    venueId,
                    dto);

            return NoContent();
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
    // DELETE VENUE
    // ADMIN ONLY
    // =====================================================
    [Authorize(Roles = "Admin")]
    [HttpDelete("{venueId:int}")]
    public async Task<IActionResult> Delete(
        int venueId)
    {
        try
        {
            await _venueService
                .DeleteAsync(
                    venueId);

            return NoContent();
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
