using CampusServicesPortal.Modules.SystemSettings.Entities;

namespace CampusServicesPortal.Modules.SystemSettings.Interfaces.Repositories;

public interface ISystemSettingRepository
{
    Task<List<SystemSetting>> GetAllAsync();

    Task<SystemSetting?> GetByKeyAsync(
        string key);

    Task<SystemSetting?> GetReservationHoldMinutesAsync();

    Task AddAsync(
        SystemSetting setting);

    Task UpdateAsync(
        SystemSetting setting);

    Task SaveChangesAsync();
}