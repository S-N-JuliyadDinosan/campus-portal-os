using CampusServicesPortal.Modules.Complaints.DTOs;

namespace CampusServicesPortal.Modules.Complaints.Interfaces.Services;

public interface IComplaintService
{
    Task<ComplaintResponseDto> CreateAsync(
        CreateComplaintDto dto);

    Task<List<ComplaintResponseDto>> GetMyComplaintsAsync();

    Task<ComplaintResponseDto?> GetByIdAsync(
        int id);

    Task<List<ComplaintResponseDto>> GetAllAsync(
        ComplaintFilterDto filter);

    Task<bool> UpdateStatusAsync(
        int id,
        UpdateComplaintStatusDto dto);
}