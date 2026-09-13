using CampusServicesPortal.Modules.Hostels.DTOs;  

namespace CampusServicesPortal.Modules.Hostels.Interfaces;

    public interface IRoomService
    {
        Task<List<RoomResponse>> GetByHostelIdAsync(
       int hostelId);

        Task<RoomResponse?> GetByIdAsync(
            int roomId);

        Task<RoomResponse> CreateAsync(
            int hostelId,
            CreateRoomRequest request);

        Task<bool> UpdateAsync(
            int roomId,
            UpdateRoomRequest request);

        Task<bool> DeleteAsync(
            int roomId);

        Task<(int Capacity, int Occupied, int Available)?>
            GetOccupancyAsync(int roomId);

        Task<List<RoomResponse>> GetFilteredAsync(
            int? hostelId,
            bool? availableOnly);
    }

