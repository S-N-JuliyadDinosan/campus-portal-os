using CampusServicesPortal.Modules.Events.DTOs.Events;
using CampusServicesPortal.Modules.Events.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Events.Controllers;

[ApiController]
[Route("api/events")]
public sealed class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(
        IEventService eventService)
    {
        _eventService = eventService;
    }


    // =====================================================
    // GET ALL EVENTS
    // PUBLIC / READ ONLY
    // =====================================================
    [HttpGet]
    public async Task<
        ActionResult<IEnumerable<EventResponseDto>>> GetAll(
            [FromQuery] EventFilterDto filter)
    {
        try
        {
            var events =
                await _eventService
                    .GetAllAsync(filter);

            return Ok(events);
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
    // GET EVENT BY ID
    // PUBLIC / READ ONLY
    // =====================================================
    [HttpGet("{eventId:int}")]
    public async Task<ActionResult<EventResponseDto>> GetById(
        int eventId)
    {
        var eventResponse =
            await _eventService
                .GetByIdAsync(eventId);

        if (eventResponse is null)
        {
            return NotFound(new
            {
                message =
                    $"Event with ID {eventId} was not found."
            });
        }

        return Ok(eventResponse);
    }


    // =====================================================
    // CREATE EVENT
    // ADMIN ONLY
    // =====================================================
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<EventResponseDto>> Create(
        [FromBody] CreateEventDto dto)
    {
        try
        {
            var eventResponse =
                await _eventService
                    .CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    eventId =
                        eventResponse.EventId
                },
                eventResponse);
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
    // UPDATE EVENT
    // ADMIN ONLY
    // =====================================================
    [Authorize(Roles = "Admin")]
    [HttpPut("{eventId:int}")]
    public async Task<IActionResult> Update(
        int eventId,
        [FromBody] UpdateEventDto dto)
    {
        try
        {
            await _eventService
                .UpdateAsync(
                    eventId,
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
    // DELETE / DEACTIVATE EVENT
    // ADMIN ONLY
    // =====================================================
    [Authorize(Roles = "Admin")]
    [HttpDelete("{eventId:int}")]
    public async Task<IActionResult> Delete(
        int eventId)
    {
        try
        {
            await _eventService
                .DeleteAsync(eventId);

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