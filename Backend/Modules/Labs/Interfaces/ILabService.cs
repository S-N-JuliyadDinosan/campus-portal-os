using CampusServicesPortal.Modules.Labs.DTOs;

namespace CampusServicesPortal.Modules.Labs.Interfaces;

    public interface ILabService
    {
        Task<List<LabResponse>> GetAllAsync();

        Task<LabResponse?> GetByIdAsync(int labId);

        Task<LabResponse> CreateAsync(
            CreateLabRequest request);

        Task<bool> UpdateAsync(
            int labId,
            UpdateLabRequest request);

        Task<bool> DeleteAsync(int labId);
    }

