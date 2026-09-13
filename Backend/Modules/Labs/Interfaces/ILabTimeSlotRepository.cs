using CampusServicesPortal.Modules.Labs.Entities;

namespace CampusServicesPortal.Modules.Labs.Interfaces;

    public interface ILabTimeSlotRepository
    {
        Task<List<LabTimeSlot>> GetByLabIdAsync(int labId);

        Task<LabTimeSlot?> GetByIdAsync(int labTimeSlotId);

        Task<LabTimeSlot> AddAsync(LabTimeSlot timeSlot);

        Task UpdateAsync(LabTimeSlot timeSlot);

        Task<bool> TimeSlotExistsAsync(
            int labId,
            string dayOfWeek,
            TimeSpan startTime,
            TimeSpan endTime,
            int? excludeTimeSlotId = null);
    }

