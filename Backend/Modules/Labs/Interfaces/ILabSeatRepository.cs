using CampusServicesPortal.Modules.Labs.Entities;

namespace CampusServicesPortal.Modules.Labs.Interfaces;

    public interface ILabSeatRepository
    {
        Task<List<LabSeat>> GetByLabIdAsync(int labId);

        Task<LabSeat?> GetByIdAsync(int labSeatId);

        Task<LabSeat> AddAsync(LabSeat labSeat);

        Task UpdateAsync(LabSeat labSeat);

        Task<bool> SeatNumberExistsAsync(
            int labId,
            string seatNumber,
            int? excludeLabSeatId = null);
    }

