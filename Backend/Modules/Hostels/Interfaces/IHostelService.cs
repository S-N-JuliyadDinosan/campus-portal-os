using CampusServicesPortal.Modules.Hostels.DTOs;

namespace CampusServicesPortal.Modules.Hostels.Interfaces;

    public interface IHostelService
    {
        Task<List<HostelResponse>> GetAllAsync();

        Task<HostelResponse?> GetByIdAsync(int hostelId);

        Task<HostelResponse> CreateAsync(
            CreateHostelRequest request);

        Task<bool> UpdateAsync(
            int hostelId,
            UpdateHostelRequest request);

        Task<bool> DeleteAsync(int hostelId);

        Task<HostelAvailabilityResponse?> GetAvailabilityAsync(
            int hostelId,
            string academicYear,
            string semester);
    }

