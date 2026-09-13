using CampusServicesPortal.Modules.Hostels.Entities;

namespace CampusServicesPortal.Modules.Hostels.Interfaces;

public interface IRoomRepository
{
    Task<List<Room>> GetByHostelIdAsync(int hostelId);

    Task<Room?> GetByIdAsync(int roomId);

    Task<Room> AddAsync(Room room);

    Task UpdateAsync(Room room);

    Task<bool> RoomNumberExistsAsync(
        int hostelId,
        string roomNumber,
        int? excludeRoomId = null);

    Task<int> GetOccupiedCountAsync(int roomId);

    Task<int> GetTotalActiveCapacityByHostelAsync(int hostelId);

    Task<List<Room>> GetFilteredAsync(
        int? hostelId,
        bool? availableOnly);
}
