using CampusServicesPortal.Modules.Hostels.Entities;

namespace CampusServicesPortal.Modules.Hostels.Interfaces;

public interface IHostelApplicationRepository
{
    Task<HostelApplication?> GetByIdAsync(
    int hostelApplicationId);

    Task<List<HostelApplication>> GetByStudentIdAsync(
        int studentId);

    Task<List<HostelApplication>> GetAllAsync(
        int? hostelId = null,
        string? status = null,
        string? academicYear = null,
        string? semester = null,
        int page = 1);

    Task<HostelApplication> AddAsync(
        HostelApplication application);

    Task UpdateAsync(
        HostelApplication application);

    Task<bool> HasActiveApplicationAsync(
        int studentId,
        string academicYear,
        string semester);

    Task<int> GetAssignedCountAsync(
        int roomId);

    Task<int> GetAssignedCountByHostelAsync(
        int hostelId,
        string academicYear,
        string semester);

}
