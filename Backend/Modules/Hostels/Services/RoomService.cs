using CampusServicesPortal.Modules.Hostels.DTOs;
using CampusServicesPortal.Modules.Hostels.Entities;
using CampusServicesPortal.Modules.Hostels.Interfaces;

namespace CampusServicesPortal.Modules.Hostels.Services;

public sealed class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IHostelRepository _hostelRepository;

    public RoomService(
        IRoomRepository roomRepository,
        IHostelRepository hostelRepository)
    {
        _roomRepository = roomRepository;
        _hostelRepository = hostelRepository;
    }

    public async Task<List<RoomResponse>> GetByHostelIdAsync(
        int hostelId)
    {
        var rooms =
            await _roomRepository.GetByHostelIdAsync(hostelId);

        return rooms
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<RoomResponse?> GetByIdAsync(
        int roomId)
    {
        var room =
            await _roomRepository.GetByIdAsync(roomId);

        return room == null
            ? null
            : MapToResponse(room);
    }

    public async Task<RoomResponse> CreateAsync(
        int hostelId,
        CreateRoomRequest request)
    {
        var hostel =
            await _hostelRepository.GetByIdAsync(hostelId);

        if (hostel == null)
        {
            throw new KeyNotFoundException(
                "Hostel not found.");
        }

        if (!hostel.IsActive)
        {
            throw new InvalidOperationException(
                "Cannot add a room to an inactive hostel.");
        }

        if (string.IsNullOrWhiteSpace(request.RoomNumber))
        {
            throw new ArgumentException(
                "Room number is required.");
        }

        if (request.Capacity <= 0)
        {
            throw new ArgumentException(
                "Room capacity must be greater than zero.");
        }

        var roomNumber = request.RoomNumber.Trim();

        var exists =
            await _roomRepository.RoomNumberExistsAsync(
                hostelId,
                roomNumber);

        if (exists)
        {
            throw new InvalidOperationException(
                "Room number already exists in this hostel.");
        }

        var room = new Room
        {
            HostelId = hostelId,
            RoomNumber = roomNumber,
            Capacity = request.Capacity,
            IsActive = true
        };

        room = await _roomRepository.AddAsync(room);

        return MapToResponse(room);
    }

    public async Task<bool> UpdateAsync(
        int roomId,
        UpdateRoomRequest request)
    {
        var room =
            await _roomRepository.GetByIdAsync(roomId);

        if (room == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.RoomNumber))
        {
            throw new ArgumentException(
                "Room number is required.");
        }

        if (request.Capacity <= 0)
        {
            throw new ArgumentException(
                "Room capacity must be greater than zero.");
        }

        var occupied =
            await _roomRepository.GetOccupiedCountAsync(roomId);

        if (request.Capacity < occupied)
        {
            throw new InvalidOperationException(
                "Room capacity cannot be lower than current occupancy.");
        }

        var roomNumber = request.RoomNumber.Trim();

        var exists =
            await _roomRepository.RoomNumberExistsAsync(
                room.HostelId,
                roomNumber,
                roomId);

        if (exists)
        {
            throw new InvalidOperationException(
                "Room number already exists in this hostel.");
        }

        room.RoomNumber = roomNumber;
        room.Capacity = request.Capacity;
        room.IsActive = request.IsActive;

        await _roomRepository.UpdateAsync(room);

        return true;
    }

    public async Task<bool> DeleteAsync(int roomId)
    {
        var room =
            await _roomRepository.GetByIdAsync(roomId);

        if (room == null)
        {
            return false;
        }

        var occupied =
            await _roomRepository.GetOccupiedCountAsync(roomId);

        if (occupied > 0)
        {
            throw new InvalidOperationException(
                "Cannot deactivate a room with active occupants.");
        }

        room.IsActive = false;

        await _roomRepository.UpdateAsync(room);

        return true;
    }

    public async Task<(int Capacity, int Occupied, int Available)?>
        GetOccupancyAsync(int roomId)
    {
        var room =
            await _roomRepository.GetByIdAsync(roomId);

        if (room == null)
        {
            return null;
        }

        var occupied =
            await _roomRepository.GetOccupiedCountAsync(roomId);

        var available =
            Math.Max(room.Capacity - occupied, 0);

        return (
            room.Capacity,
            occupied,
            available);
    }

    public async Task<List<RoomResponse>> GetFilteredAsync(
        int? hostelId,
        bool? availableOnly)
    {
        var rooms =
            await _roomRepository.GetFilteredAsync(
                hostelId,
                availableOnly);

        return rooms
            .Select(MapToResponse)
            .ToList();
    }

    private static RoomResponse MapToResponse(Room room)
    {
        return new RoomResponse
        {
            RoomId = room.RoomId,
            HostelId = room.HostelId,
            RoomNumber = room.RoomNumber,
            Capacity = room.Capacity,
            IsActive = room.IsActive,
            CreatedAt = room.CreatedAt
        };
    }
}