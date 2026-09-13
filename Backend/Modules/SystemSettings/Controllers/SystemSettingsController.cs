using CampusServicesPortal.Modules.SystemSettings.DTOs;
using CampusServicesPortal.Modules.SystemSettings.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.SystemSettings.Controllers;

[ApiController]
[Route("api/admin/system-settings")]
[Authorize(Roles = "Admin")]
public sealed class SystemSettingsController : ControllerBase
{
    private readonly ISystemSettingService _systemSettingService;

    public SystemSettingsController(
        ISystemSettingService systemSettingService)
    {
        _systemSettingService = systemSettingService;
    }

    // GET: api/admin/system-settings/reservation-hold-minutes
    [HttpGet("reservation-hold-minutes")]
    public async Task<
        ActionResult<ReservationHoldMinutesDto>>
        GetReservationHoldMinutes()
    {
        var result =
            await _systemSettingService
                .GetReservationHoldMinutesAsync();

        return Ok(result);
    }

    // PUT: api/admin/system-settings/reservation-hold-minutes
    [HttpPut("reservation-hold-minutes")]
    public async Task<
        ActionResult<ReservationHoldMinutesDto>>
        UpdateReservationHoldMinutes(
            [FromBody] ReservationHoldMinutesDto dto)
    {
        var result =
            await _systemSettingService
                .UpdateReservationHoldMinutesAsync(dto);

        return Ok(result);
    }

    // GET: api/admin/system-settings
    [HttpGet]
    public async Task<
        ActionResult<List<SystemSettingResponseDto>>>
        GetAll()
    {
        var settings =
            await _systemSettingService
                .GetAllAsync();

        return Ok(settings);
    }

    // GET: api/admin/system-settings/{key}
    [HttpGet("{key}")]
    public async Task<
        ActionResult<SystemSettingResponseDto>>
        GetByKey(string key)
    {
        var setting =
            await _systemSettingService
                .GetByKeyAsync(key);

        if (setting is null)
        {
            return NotFound(new
            {
                message =
                    $"System setting with key '{key}' was not found."
            });
        }

        return Ok(setting);
    }

    // PUT: api/admin/system-settings/{key}
    [HttpPut("{key}")]
    public async Task<
        ActionResult<SystemSettingResponseDto>>
        Update(
            string key,
            [FromBody] UpdateSystemSettingDto dto)
    {
        var setting =
            await _systemSettingService
                .UpdateAsync(key, dto);

        return Ok(setting);
    }
}