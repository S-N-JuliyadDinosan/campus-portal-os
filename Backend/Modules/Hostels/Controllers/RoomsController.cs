using CampusServicesPortal.Modules.Hostels.DTOs;
using CampusServicesPortal.Modules.Hostels.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Hostels.Controllers;

[ApiController]
[Route("api")]
public sealed class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    // GET: /api/hostels/1/rooms
    [HttpGet("hostels/{id:int}/rooms")]
    public async Task<IActionResult> GetByHostel(int id)
    {
        var rooms =
            await _roomService.GetByHostelIdAsync(id);

        return Ok(rooms);
    }

    // POST: /api/hostels/1/rooms
    [HttpPost("hostels/{hostelId:int}/rooms")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        int hostelId,
        [FromBody] CreateRoomRequest request)
    {
        try
        {
            var room =
                await _roomService.CreateAsync(
                    hostelId,
                    request);

            return StatusCode(
                StatusCodes.Status201Created,
                room);
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

    // GET: /api/rooms/1
    [HttpGet("rooms/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var room =
            await _roomService.GetByIdAsync(id);

        if (room == null)
        {
            return NotFound(new
            {
                message = "Room not found."
            });
        }

        return Ok(room);
    }

    // GET:
    // /api/rooms?hostelId=1&availableOnly=true
    [HttpGet("rooms")]
    public async Task<IActionResult> GetFiltered(
        [FromQuery] int? hostelId,
        [FromQuery] bool? availableOnly)
    {
        var rooms =
            await _roomService.GetFilteredAsync(
                hostelId,
                availableOnly);

        return Ok(rooms);
    }

    // PUT: /api/rooms/1
    [HttpPut("rooms/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateRoomRequest request)
    {
        try
        {
            var updated =
                await _roomService.UpdateAsync(
                    id,
                    request);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Room not found."
                });
            }

            return Ok(new
            {
                message = "Room updated successfully."
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

    // DELETE: /api/rooms/1
    [HttpDelete("rooms/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted =
                await _roomService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Room not found."
                });
            }

            return Ok(new
            {
                message = "Room deactivated successfully."
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

    // GET: /api/rooms/1/occupancy
    [HttpGet("rooms/{id:int}/occupancy")]
    public async Task<IActionResult> GetOccupancy(int id)
    {
        var occupancy =
            await _roomService.GetOccupancyAsync(id);

        if (occupancy == null)
        {
            return NotFound(new
            {
                message = "Room not found."
            });
        }

        return Ok(new
        {
            roomId = id,
            capacity = occupancy.Value.Capacity,
            occupied = occupancy.Value.Occupied,
            available = occupancy.Value.Available
        });
    }
}