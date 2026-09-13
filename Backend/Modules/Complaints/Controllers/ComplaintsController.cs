using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Common.Security;
using CampusServicesPortal.Modules.Complaints.DTOs;
using CampusServicesPortal.Modules.Complaints.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Complaints.Controllers;

[ApiController]
[Route("api/complaints")]
[Authorize]
public sealed class ComplaintsController(
    IComplaintService service,
    ICurrentUserService currentUser)
    : ControllerBase
{
    // POST: /api/complaints
    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Create(
        CreateComplaintDto dto)
    {
        var result = await service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.ComplaintId },
            result);
    }

    // GET: /api/complaints/me
    [HttpGet("me")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyComplaints()
    {
        var result = await service.GetMyComplaintsAsync();

        return Ok(result);
    }

    // GET: /api/complaints/student/{studentId}
    [HttpGet("student/{studentId:int}")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> GetStudentComplaints(int studentId)
    {
        if (User.IsInRole("Student"))
        {
            if (!currentUser.StudentId.HasValue ||
                currentUser.StudentId.Value != studentId)
            {
                return Forbid();
            }

            return Ok(await service.GetMyComplaintsAsync());
        }

        return Ok(await service.GetAllAsync(
            new ComplaintFilterDto { StudentId = studentId }));
    }

    // GET: /api/complaints/{id}
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    // GET: /api/complaints?status=&categoryId=&studentId=
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] ComplaintFilterDto filter)
    {
        var result = await service.GetAllAsync(filter);

        return Ok(result);
    }

    // PUT: /api/complaints/{id}/status
    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateComplaintStatusDto dto)
    {
        var updated =
            await service.UpdateStatusAsync(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }
}