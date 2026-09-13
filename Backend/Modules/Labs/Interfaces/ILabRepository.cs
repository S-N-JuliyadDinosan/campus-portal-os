using CampusServicesPortal.Modules.Labs.Entities;

namespace CampusServicesPortal.Modules.Labs.Interfaces;

public interface ILabRepository
{
    Task<List<Lab>> GetAllAsync();

    Task<Lab?> GetByIdAsync(int labId);

    Task<Lab> AddAsync(Lab lab);

    Task UpdateAsync(Lab lab);

    Task<bool> CodeExistsAsync(
        string code,
        int? excludeLabId = null);
}