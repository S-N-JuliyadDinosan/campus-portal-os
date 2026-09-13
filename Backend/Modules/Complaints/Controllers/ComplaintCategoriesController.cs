using CampusServicesPortal.Modules.Complaints.DTOs;
using CampusServicesPortal.Modules.Complaints.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Complaints.Controllers;

[ApiController]
[Route("api/complaint-categories")]
[Authorize]
public sealed class ComplaintCategoriesController(
    IComplaintCategoryService service)
    : ControllerBase
{
    // GET: /api/complaint-categories
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await service.GetAllAsync();

        return Ok(result);
    }

    // GET: /api/complaint-categories/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    // POST: /api/complaint-categories
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        ComplaintCategoryCreateDto dto)
    {
        var result = await service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.ComplaintCategoryId },
            result);
    }

    // PUT: /api/complaint-categories/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        ComplaintCategoryUpdateDto dto)
    {
        var updated = await service.UpdateAsync(id, dto);

        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    // DELETE: /api/complaint-categories/{id}
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await service.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
