using CampusServicesPortal.Modules.Hostels.Entities;

namespace CampusServicesPortal.Modules.Hostels.Interfaces;

public interface IHostelRepository
{
    Task<List<Hostel>> GetAllAsync();

    Task<Hostel?> GetByIdAsync(int hostelId);

    Task<Hostel?> GetByNameAsync(string name);

    Task<Hostel> AddAsync(Hostel hostel);

    Task UpdateAsync(Hostel hostel);

    Task DeleteAsync(Hostel hostel);

    Task<bool> NameExistsAsync(
        string name,
        int? excludeHostelId = null);
}
