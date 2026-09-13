using CampusServicesPortal.Modules.Hostels.DTOs;

namespace CampusServicesPortal.Modules.Hostels.Interfaces;

public interface IHostelApplicationService
{
    Task<HostelApplicationResponse> CreateAsync(
   CreateHostelApplicationRequest request);

    Task<HostelApplicationResponse?> GetByIdAsync(
        int hostelApplicationId);

    Task<List<HostelApplicationResponse>> GetByStudentIdAsync(
        int studentId);

    Task<List<HostelApplicationResponse>> GetAllAsync(
        int? hostelId = null,
        string? status = null,
        string? academicYear = null,
        string? semester = null,
        int page = 1);

    Task<bool> UpdateStatusAsync(
        int hostelApplicationId,
        UpdateHostelApplicationStatusRequest request,
        int? reviewedByUserId = null);

    Task<bool> AssignRoomAsync(
        int hostelApplicationId,
        AssignRoomRequest request,
        int? reviewedByUserId = null);

    Task<bool> UnassignRoomAsync(
        int hostelApplicationId,
        int? reviewedByUserId = null);

    Task<bool> CancelAsync(int hostelApplicationId);
}
