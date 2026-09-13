using CampusServicesPortal.Modules.Certificates.DTOs;
using CampusServicesPortal.Modules.Certificates.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Certificates.Controllers;

[ApiController]
[Route("api/certificate-requests")]
[Authorize]
public sealed class CertificateRequestsController(
    ICertificateService service)
    : ControllerBase
{
    // =============================================
    // STUDENT
    // =============================================

    // POST /api/certificate-requests
    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<CertificateRequestDto>> Create(
        [FromBody] CreateCertificateRequestDto dto)
    {
        var result =
            await service.CreateRequestAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.CertificateRequestId
            },
            result);
    }


    // GET /api/certificate-requests/me
    [HttpGet("me")]
    [Authorize(Roles = "Student")]
    public async Task<
        ActionResult<List<CertificateRequestDto>>> GetMine()
    {
        var result =
            await service.GetMyRequestsAsync();

        return Ok(result);
    }


    // =============================================
    // STUDENT / ADMIN
    // =============================================

    // GET /api/certificate-requests/student/1
    [HttpGet("student/{studentId:int}")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<
        ActionResult<List<CertificateRequestDto>>> GetByStudent(
        int studentId)
    {
        var result =
            await service.GetRequestsByStudentIdAsync(
                studentId);

        return Ok(result);
    }


    // GET /api/certificate-requests/1
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<ActionResult<CertificateRequestDto>>
        GetById(int id)
    {
        var result =
            await service.GetRequestByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }


    // =============================================
    // ADMIN
    // =============================================

    // GET /api/certificate-requests
    // GET /api/certificate-requests?status=Pending&page=1
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<
        ActionResult<List<CertificateRequestDto>>> GetAll(
        [FromQuery] string? status = null,
        [FromQuery] int page = 1)
    {
        var result =
            await service.GetAllRequestsAsync(
                status,
                page);

        return Ok(result);
    }


    // PUT /api/certificate-requests/1/status
    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CertificateRequestDto>>
        UpdateStatus(
            int id,
            [FromBody] UpdateCertificateRequestDto dto)
    {
        var result =
            await service.UpdateRequestStatusAsync(
                id,
                dto);

        return Ok(result);
    }
}