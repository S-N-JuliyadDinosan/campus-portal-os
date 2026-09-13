using CampusServicesPortal.Modules.Certificates.DTOs;
using CampusServicesPortal.Modules.Certificates.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Certificates.Controllers;

[ApiController]
[Route("api/certificate-types")]
[Authorize]
public sealed class CertificateTypesController(
    ICertificateService service)
    : ControllerBase
{
    // GET /api/certificate-types
    [HttpGet]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await service.GetCertificateTypesAsync());
    }

    // Recommended supporting endpoint
    // GET /api/certificate-types/{id}
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await service.GetCertificateTypeByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    // BRD endpoint: POST /api/certificate-types
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateCertificateTypeDto dto)
    {
        var result = await service.CreateCertificateTypeAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.CertificateTypeId },
            result);
    }

    // BRD endpoint: PUT /api/certificate-types/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateCertificateTypeDto dto)
    {
        return Ok(await service.UpdateCertificateTypeAsync(id, dto));
    }

    // BRD endpoint: DELETE /api/certificate-types/{id}
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOrDeactivate(int id)
    {
        await service.DeleteOrDeactivateCertificateTypeAsync(id);
        return NoContent();
    }
}
