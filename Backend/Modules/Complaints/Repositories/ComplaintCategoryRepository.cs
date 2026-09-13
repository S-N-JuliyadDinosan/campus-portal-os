using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Complaints.Entities;
using CampusServicesPortal.Modules.Complaints.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Complaints.Repositories;

public sealed class ComplaintCategoryRepository(
    ApplicationDbContext context)
    : IComplaintCategoryRepository
{
    public IQueryable<ComplaintCategory> Query()
    {
        return context.ComplaintCategories
            .AsNoTracking();
    }

    public async Task<List<ComplaintCategory>> GetAllAsync()
    {
        return await context.ComplaintCategories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<ComplaintCategory?> GetByIdAsync(int id)
    {
        return await context.ComplaintCategories
            .FirstOrDefaultAsync(
                x => x.ComplaintCategoryId == id);
    }

    public async Task AddAsync(
        ComplaintCategory category)
    {
        await context.ComplaintCategories.AddAsync(category);
    }

    public Task UpdateAsync(
        ComplaintCategory category)
    {
        context.ComplaintCategories.Update(category);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(
        ComplaintCategory category)
    {
        context.ComplaintCategories.Remove(category);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}