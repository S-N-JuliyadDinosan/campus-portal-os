using CampusServicesPortal.Common.Pagination;
using CampusServicesPortal.Common.Responses;
using CampusServicesPortal.Modules.Students.DTOs;
using CampusServicesPortal.Modules.Students.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Students.Controllers;

[ApiController]
[Route("api/student-master")]
public sealed class StudentMasterController(IStudentMasterService service) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("check")]
    public async Task<ActionResult<ApiResponse<StudentMasterCheckResponse>>> CheckByQuery(
        [FromQuery] string indexNumber,
        CancellationToken cancellationToken)
    {
        var result = await service.CheckAsync(indexNumber, cancellationToken);
        return Ok(ApiResponse<StudentMasterCheckResponse>.Ok(result));
    }

    // API-026
    [AllowAnonymous]
    [HttpGet("{indexNumber}")]
    public async Task<ActionResult<ApiResponse<StudentMasterCheckResponse>>> Check(
        string indexNumber,
        CancellationToken cancellationToken)
    {
        var result = await service.CheckAsync(indexNumber, cancellationToken);
        return Ok(ApiResponse<StudentMasterCheckResponse>.Ok(result));
    }

    // API-027
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<StudentMasterResponse>>>> Search(
        [FromQuery] string? search,
        [FromQuery] int? facultyId,
        [FromQuery] int? intakeYear,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await service.SearchAsync(search, facultyId, intakeYear, isActive, page, cancellationToken);
        return Ok(ApiResponse<PagedResult<StudentMasterResponse>>.Ok(result));
    }

    // API-028
    [Authorize(Roles = "Admin")]
    [HttpGet("records/{id:int}")]
    public async Task<ActionResult<ApiResponse<StudentMasterResponse>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<StudentMasterResponse>.Ok(result));
    }

    // API-029
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<StudentMasterResponse>>> Create(
        [FromBody] CreateStudentMasterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return Created($"/api/student-master/records/{result.StudentMasterId}",
            ApiResponse<StudentMasterResponse>.Ok(result, "Student master record created."));
    }

    // API-030
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<StudentMasterResponse>>> Update(
        int id,
        [FromBody] UpdateStudentMasterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<StudentMasterResponse>.Ok(result, "Student master record updated."));
    }

    // API-031
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok("Student master record permanently deleted."));
    }

    // API-032
    [Authorize(Roles = "Admin")]
    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult<ApiResponse<StudentMasterImportResponse>>> Import(
    IFormFile file,
    CancellationToken cancellationToken)
    {
        var result = await service.ImportCsvAsync(file, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(
                ApiResponse<StudentMasterImportResponse>.Fail(
                    "CSV validation failed. No records were changed.",
                    result.Errors));
        }

        return Ok(
            ApiResponse<StudentMasterImportResponse>.Ok(
                result,
                "Student master CSV imported successfully."));
    }
}
