using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Modules.Complaints.Entities;

namespace CampusServicesPortal.Modules.Complaints.Interfaces.Repositories;

public interface IComplaintRepository
{
    IQueryable<Complaint> Query();

    Task<List<Complaint>> GetAllAsync();

    Task<Complaint?> GetByIdAsync(int id);

    Task<List<Complaint>> GetStudentComplaintsAsync(
        int studentId);

    Task<List<Complaint>> GetByStatusAsync(
        ComplaintStatus status);

    Task<List<Complaint>> GetByCategoryAsync(
        int categoryId);

    Task AddAsync(Complaint complaint);

    Task UpdateAsync(Complaint complaint);

    Task DeleteAsync(Complaint complaint);

    Task SaveChangesAsync();
}