using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Complaints.Entities;
using CampusServicesPortal.Modules.Complaints.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Complaints.Repositories;

public sealed class ComplaintRepository(
    ApplicationDbContext context)
    : IComplaintRepository
{
    public IQueryable<Complaint> Query()
    {
        return context.Complaints
            .AsNoTracking()
            .Include(x => x.ComplaintCategory)
            .Include(x => x.Student);
    }

    public async Task<List<Complaint>> GetAllAsync()
    {
        return await context.Complaints
            .AsNoTracking()
            .Include(x => x.ComplaintCategory)
            .Include(x => x.Student)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Complaint?> GetByIdAsync(int id)
    {
        return await context.Complaints
            .Include(x => x.ComplaintCategory)
            .Include(x => x.Student)
            .FirstOrDefaultAsync(
                x => x.ComplaintId == id);
    }

    public async Task<List<Complaint>> GetStudentComplaintsAsync(
        int studentId)
    {
        return await context.Complaints
            .AsNoTracking()
            .Include(x => x.ComplaintCategory)
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Complaint>> GetByStatusAsync(
        ComplaintStatus status)
    {
        return await context.Complaints
            .AsNoTracking()
            .Include(x => x.ComplaintCategory)
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Complaint>> GetByCategoryAsync(
        int categoryId)
    {
        return await context.Complaints
            .AsNoTracking()
            .Include(x => x.ComplaintCategory)
            .Where(x =>
                x.ComplaintCategoryId == categoryId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(
        Complaint complaint)
    {
        await context.Complaints.AddAsync(complaint);
    }

    public Task UpdateAsync(
        Complaint complaint)
    {
        context.Complaints.Update(complaint);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(
        Complaint complaint)
    {
        context.Complaints.Remove(complaint);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
