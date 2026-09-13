using CampusServicesPortal.Common.Responses;
using CampusServicesPortal.Common.Security;
using CampusServicesPortal.Modules.Students.DTOs;
using CampusServicesPortal.Modules.Students.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Students.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/admins")]
public sealed class AdminAccountsController(
    IAdminAccountService service,
    ICurrentUserService currentUser) : ControllerBase
{
    // API-020
    [HttpPost]
    public async Task<ActionResult<ApiResponse<AdminAccountResponse>>> Create(
        [FromBody] CreateAdminRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return Created($"/api/admin/admins/{result.UserId}",
            ApiResponse<AdminAccountResponse>.Ok(result, "Administrator created."));
    }

    // API-021
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<AdminAccountResponse>>>> List(
        CancellationToken cancellationToken)
    {
        var result = await service.ListAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<AdminAccountResponse>>.Ok(result));
    }

    // API-022
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<AdminAccountResponse>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<AdminAccountResponse>.Ok(result));
    }

    // API-023
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<AdminAccountResponse>>> Update(
        int id,
        [FromBody] UpdateAdminRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<AdminAccountResponse>.Ok(result, "Administrator updated."));
    }

    // API-024
    [HttpPut("{id:int}/deactivate")]
    public async Task<ActionResult<ApiResponse<object>>> Deactivate(
        int id,
        CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("Authenticated user id is missing.");
        await service.DeactivateAsync(id, currentUserId, cancellationToken);
        return Ok(ApiResponse.Ok("Administrator deactivated."));
    }

    // API-025
    [HttpPut("{id:int}/reactivate")]
    public async Task<ActionResult<ApiResponse<object>>> Reactivate(
        int id,
        CancellationToken cancellationToken)
    {
        await service.ReactivateAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok("Administrator reactivated."));
    }
}
