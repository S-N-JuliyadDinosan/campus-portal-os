using CampusServicesPortal.Modules.Labs.DTOs;

namespace CampusServicesPortal.Modules.Labs.Interfaces;

    public interface ILabTimeSlotService
    {
        Task<List<LabTimeSlotResponse>> GetByLabIdAsync(
       int labId);

        Task<LabTimeSlotResponse?> GetByIdAsync(
            int labTimeSlotId);

        Task<LabTimeSlotResponse> CreateAsync(
            int labId,
            CreateLabTimeSlotRequest request);

        Task<bool> UpdateAsync(
            int labTimeSlotId,
            UpdateLabTimeSlotRequest request);

        Task<bool> DeleteAsync(int labTimeSlotId);

        Task<List<LabTimeSlotResponse>> GetAvailableByDateAsync(
            int labId,
            DateOnly date);
    }

