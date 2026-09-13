using CampusServicesPortal.Modules.Labs.DTOs;

namespace CampusServicesPortal.Modules.Labs.Interfaces;

    public interface ILabSeatService
    {
        Task<List<LabSeatResponse>> GetByLabIdAsync(
        int labId);

        Task<LabSeatResponse?> GetByIdAsync(
            int labSeatId);

        Task<LabSeatResponse> CreateAsync(
            int labId,
            CreateLabSeatRequest request);

        Task<bool> UpdateAsync(
            int labSeatId,
            UpdateLabSeatRequest request);

        Task<bool> DeleteAsync(int labSeatId);

        Task<List<LabSeatResponse>> GetAvailableSeatsAsync(
            int labId,
            int labTimeSlotId,
            DateOnly bookingDate);
    }

