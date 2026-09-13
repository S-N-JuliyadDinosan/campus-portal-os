using CampusServicesPortal.Modules.SystemSettings.DTOs;
using CampusServicesPortal.Modules.SystemSettings.Entities;
using CampusServicesPortal.Modules.SystemSettings.Interfaces.Repositories;
using CampusServicesPortal.Modules.SystemSettings.Interfaces.Services;

namespace CampusServicesPortal.Modules.SystemSettings.Services;

public sealed class SystemSettingService
    : ISystemSettingService
{
    private readonly ISystemSettingRepository _repository;

    public SystemSettingService(
        ISystemSettingRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<SystemSettingResponseDto>>
        GetAllAsync()
    {
        var settings =
            await _repository.GetAllAsync();

        return settings
            .Select(MapToResponseDto)
            .ToList();
    }

    public async Task<SystemSettingResponseDto?>
        GetByKeyAsync(string key)
    {
        var setting =
            await _repository.GetByKeyAsync(key);

        if (setting is null)
        {
            return null;
        }

        return MapToResponseDto(setting);
    }

    public async Task<ReservationHoldMinutesDto>
        GetReservationHoldMinutesAsync()
    {
        var setting =
            await _repository
                .GetReservationHoldMinutesAsync();

        if (setting is null)
        {
            throw new KeyNotFoundException(
                "Reservation hold minutes setting was not found.");
        }

        if (!int.TryParse(
                setting.SettingValue,
                out var minutes))
        {
            throw new InvalidOperationException(
                "Reservation hold minutes setting contains an invalid value.");
        }

        return new ReservationHoldMinutesDto
        {
            Minutes = minutes
        };
    }

    public async Task<SystemSettingResponseDto>
        UpdateAsync(
            string key,
            UpdateSystemSettingDto dto)
    {
        var setting =
            await _repository.GetByKeyAsync(key);

        if (setting is null)
        {
            throw new KeyNotFoundException(
                $"System setting with key '{key}' was not found.");
        }

        setting.SettingValue = dto.SettingValue;

        await _repository.UpdateAsync(setting);
        await _repository.SaveChangesAsync();

        return MapToResponseDto(setting);
    }

    public async Task<ReservationHoldMinutesDto>
        UpdateReservationHoldMinutesAsync(
            ReservationHoldMinutesDto dto)
    {
        if (dto.Minutes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dto.Minutes),
                "Reservation hold minutes must be greater than zero.");
        }

        var setting =
            await _repository
                .GetReservationHoldMinutesAsync();

        if (setting is null)
        {
            throw new KeyNotFoundException(
                "Reservation hold minutes setting was not found.");
        }

        setting.SettingValue =
            dto.Minutes.ToString();

        await _repository.UpdateAsync(setting);
        await _repository.SaveChangesAsync();

        return new ReservationHoldMinutesDto
        {
            Minutes = dto.Minutes
        };
    }

    private static SystemSettingResponseDto
        MapToResponseDto(
            SystemSetting setting)
    {
        return new SystemSettingResponseDto
        {
            SystemSettingId =
                setting.SystemSettingId,

            SettingKey =
                setting.SettingKey,

            SettingValue =
                setting.SettingValue,

            Description =
                setting.Description,

            UpdatedByUserId =
                setting.UpdatedByUserId,

            UpdatedAt =
                setting.UpdatedAt
        };
    }
}