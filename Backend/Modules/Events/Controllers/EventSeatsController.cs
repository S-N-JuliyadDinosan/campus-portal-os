using CampusServicesPortal.Modules.Events.DTOs.EventSeats;
using CampusServicesPortal.Modules.Events.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Events.Controllers;

[ApiController]
[Route("api")]
public sealed class EventSeatsController : ControllerBase
{
    private readonly IEventSeatService _eventSeatService;

    public EventSeatsController(
        IEventSeatService eventSeatService)
    {
        _eventSeatService = eventSeatService;
    }


    // =====================================================
    // GET ALL SEATS FOR AN EVENT
    // =====================================================
    // GET: /api/events/5/seats
    [HttpGet("events/{eventId:int}/seats")]
    public async Task<
        ActionResult<IEnumerable<EventSeatResponseDto>>>
        GetByEventId(
            int eventId)
    {
        try
        {
            var seats =
                await _eventSeatService
                    .GetByEventIdAsync(
                        eventId);

            return Ok(seats);
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
    // GET EVENT SEAT AVAILABILITY
    // =====================================================
    // GET: /api/events/5/seats/availability
    [HttpGet("events/{eventId:int}/seats/availability")]
    public async Task<
        ActionResult<IEnumerable<SeatAvailabilityResponseDto>>>
        GetAvailability(
            int eventId)
    {
        try
        {
            var seats =
                await _eventSeatService
                    .GetAvailabilityAsync(
                        eventId);

            return Ok(seats);
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
    // CREATE EVENT SEAT
    // ADMIN ONLY
    // =====================================================
    // POST: /api/events/5/seats
    [Authorize(Roles = "Admin")]
    [HttpPost("events/{eventId:int}/seats")]
    public async Task<ActionResult<EventSeatResponseDto>>
        Create(
            int eventId,
            [FromBody] CreateEventSeatDto dto)
    {
        try
        {
            var seat =
                await _eventSeatService
                    .CreateAsync(
                        eventId,
                        dto);

            return CreatedAtAction(
                nameof(GetByEventId),
                new
                {
                    eventId
                },
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
    // UPDATE EVENT SEAT
    // ADMIN ONLY
    // =====================================================
    // PUT: /api/event-seats/10
    [Authorize(Roles = "Admin")]
    [HttpPut("event-seats/{seatId:int}")]
    public async Task<IActionResult> Update(
        int seatId,
        [FromBody] UpdateEventSeatDto dto)
    {
        try
        {
            await _eventSeatService
                .UpdateAsync(
                    seatId,
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
    // DELETE / DEACTIVATE EVENT SEAT
    // ADMIN ONLY
    // =====================================================
    // DELETE: /api/event-seats/10
    [Authorize(Roles = "Admin")]
    [HttpDelete("event-seats/{seatId:int}")]
    public async Task<IActionResult> Delete(
        int seatId)
    {
        try
        {
            await _eventSeatService
                .DeleteAsync(
                    seatId);

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