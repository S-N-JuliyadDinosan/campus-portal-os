using CampusServicesPortal.Modules.Complaints.DTOs;
using CampusServicesPortal.Modules.Complaints.Entities;

namespace CampusServicesPortal.Modules.Complaints.Interfaces.Services;

public interface IComplaintCategoryService
{
    Task<List<ComplaintCategoryResponseDto>> GetAllAsync();

    Task<ComplaintCategoryResponseDto?> GetByIdAsync(
        int id);

    Task<ComplaintCategoryResponseDto> CreateAsync(
        ComplaintCategoryCreateDto dto);

    Task<bool> UpdateAsync(
        int id,
        ComplaintCategoryUpdateDto dto);

    Task<bool> DeleteAsync(
        int id);
}