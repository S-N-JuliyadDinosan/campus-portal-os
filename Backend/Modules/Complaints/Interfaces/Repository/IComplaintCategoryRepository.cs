using CampusServicesPortal.Modules.Complaints.Entities;

namespace CampusServicesPortal.Modules.Complaints.Interfaces.Repositories;

public interface IComplaintCategoryRepository
{
    IQueryable<ComplaintCategory> Query();

    Task<List<ComplaintCategory>> GetAllAsync();

    Task<ComplaintCategory?> GetByIdAsync(int id);

    Task AddAsync(ComplaintCategory category);

    Task UpdateAsync(ComplaintCategory category);

    Task DeleteAsync(ComplaintCategory category);

    Task SaveChangesAsync();
}