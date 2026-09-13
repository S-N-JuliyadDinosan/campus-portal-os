using CampusServicesPortal.Modules.SystemSettings.DTOs;

namespace CampusServicesPortal.Modules.SystemSettings.Interfaces.Services;

public interface ISystemSettingService
{
    Task<List<SystemSettingResponseDto>>
        GetAllAsync();

    Task<SystemSettingResponseDto?>
        GetByKeyAsync(
            string key);

    Task<ReservationHoldMinutesDto>
        GetReservationHoldMinutesAsync();

    Task<SystemSettingResponseDto>
        UpdateAsync(
            string key,
            UpdateSystemSettingDto dto);

    Task<ReservationHoldMinutesDto>
        UpdateReservationHoldMinutesAsync(
            ReservationHoldMinutesDto dto);
}