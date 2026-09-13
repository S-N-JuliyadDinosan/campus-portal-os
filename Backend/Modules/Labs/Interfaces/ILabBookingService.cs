using CampusServicesPortal.Modules.Labs.DTOs;

namespace CampusServicesPortal.Modules.Labs.Interfaces;

    public interface ILabBookingService
    {
        Task<LabBookingResponse> CreateAsync(
       CreateLabBookingRequest request);

        Task<LabBookingResponse?> GetByIdAsync(
            int labBookingId);

        Task<List<LabBookingResponse>> GetByStudentIdAsync(
            int studentId);

        Task<List<LabBookingResponse>> GetAllAsync(
            int? labId = null,
            int? studentId = null,
            string? status = null,
            DateOnly? date = null,
            int page = 1);

        Task<bool> ConfirmAsync(int labBookingId);

        Task<bool> CancelAsync(int labBookingId);
    }

