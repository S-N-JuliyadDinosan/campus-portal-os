using CampusServicesPortal.Common.Responses;
using CampusServicesPortal.Modules.Students.DTOs;
using CampusServicesPortal.Modules.Students.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Students.Controllers;

[ApiController]
[Route("api/faculties")]
public sealed class FacultiesController(IFacultyService service) : ControllerBase
{
    // API-033
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<FacultyResponse>>>> List(
        CancellationToken cancellationToken)
    {
        var result = await service.ListActiveAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<FacultyResponse>>.Ok(result));
    }

    // API-034
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<FacultyResponse>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<FacultyResponse>.Ok(result));
    }

    // API-035
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<FacultyResponse>>> Create(
        [FromBody] CreateFacultyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return Created($"/api/faculties/{result.FacultyId}",
            ApiResponse<FacultyResponse>.Ok(result, "Faculty created."));
    }

    // API-036
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<FacultyResponse>>> Update(
        int id,
        [FromBody] UpdateFacultyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<FacultyResponse>.Ok(result, "Faculty updated."));
    }

    // API-037
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<FacultyDeleteResponse>>> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await service.DeleteOrDeactivateAsync(id, cancellationToken);
        return Ok(ApiResponse<FacultyDeleteResponse>.Ok(result, result.Message));
    }
}
