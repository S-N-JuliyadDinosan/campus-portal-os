using CampusServicesPortal.Modules.Complaints.DTOs;
using CampusServicesPortal.Modules.Complaints.Entities;
using CampusServicesPortal.Modules.Complaints.Interfaces.Repositories;
using CampusServicesPortal.Modules.Complaints.Interfaces.Services;

namespace CampusServicesPortal.Modules.Complaints.Services;

public sealed class ComplaintCategoryService(
    IComplaintCategoryRepository repository)
    : IComplaintCategoryService
{
    public async Task<List<ComplaintCategoryResponseDto>> GetAllAsync()
    {
        var categories = await repository.GetAllAsync();

        return categories
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<ComplaintCategoryResponseDto?> GetByIdAsync(
        int id)
    {
        var category = await repository.GetByIdAsync(id);

        return category is null
            ? null
            : MapToResponse(category);
    }

    public async Task<ComplaintCategoryResponseDto> CreateAsync(
       ComplaintCategoryCreateDto dto)
    {
        var category = new ComplaintCategory
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            IsActive = true
        };

        await repository.AddAsync(category);
        await repository.SaveChangesAsync();

        return MapToResponse(category);
    }

    public async Task<ComplaintCategoryResponseDto?> UpdateAsync(
        int id,
        ComplaintCategoryUpdateDto dto)
    {
        var category = await repository.GetByIdAsync(id);

        if (category is null)
        {
            return null;
        }

        category.Name = dto.Name.Trim();
        category.Description = dto.Description?.Trim();
        category.IsActive = dto.IsActive;

        await repository.UpdateAsync(category);
        await repository.SaveChangesAsync();

        return MapToResponse(category);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await repository.GetByIdAsync(id);

        if (category is null)
        {
            return false;
        }

        await repository.DeleteAsync(category);
        await repository.SaveChangesAsync();

        return true;
    }

    private static ComplaintCategoryResponseDto MapToResponse(
        ComplaintCategory category)
    {
        return new ComplaintCategoryResponseDto
        {
            ComplaintCategoryId = category.ComplaintCategoryId,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive
        };
    }
}
