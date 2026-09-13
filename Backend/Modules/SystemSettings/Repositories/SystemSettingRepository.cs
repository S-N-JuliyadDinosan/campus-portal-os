using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.SystemSettings.Entities;
using CampusServicesPortal.Modules.SystemSettings.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.SystemSettings.Repositories;

public sealed class SystemSettingRepository
    : ISystemSettingRepository
{
    private readonly ApplicationDbContext _context;

    public SystemSettingRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SystemSetting>>
        GetAllAsync()
    {
        return await _context.SystemSettings
            .AsNoTracking()
            .OrderBy(x => x.SettingKey)
            .ToListAsync();
    }

    public async Task<SystemSetting?>
        GetByKeyAsync(
            string key)
    {
        return await _context.SystemSettings
            .FirstOrDefaultAsync(x =>
                x.SettingKey == key);
    }

    public async Task<SystemSetting?>
        GetReservationHoldMinutesAsync()
    {
        return await _context.SystemSettings
            .FirstOrDefaultAsync(x =>
                x.SettingKey == "ReservationHoldMinutes");
    }

    public async Task AddAsync(
        SystemSetting setting)
    {
        await _context.SystemSettings
            .AddAsync(setting);
    }

    public async Task UpdateAsync(
        SystemSetting setting)
    {
        _context.SystemSettings
            .Update(setting);

        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}