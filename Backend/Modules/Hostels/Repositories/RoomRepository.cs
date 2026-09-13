using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Hostels.Entities;
using CampusServicesPortal.Modules.Hostels.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Hostels.Repositories;

public sealed class RoomRepository : IRoomRepository
{
    private readonly ApplicationDbContext _context;

    public RoomRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // GET ROOMS BY HOSTEL
    // =========================================================
    public async Task<List<Room>> GetByHostelIdAsync(
        int hostelId)
    {
        return await _context.Rooms
            .AsNoTracking()
            .Where(x =>
                x.HostelId == hostelId)
            .OrderBy(x =>
                x.RoomNumber)
            .ToListAsync();
    }


    // =========================================================
    // GET ROOM BY ID
    // =========================================================
    public async Task<Room?> GetByIdAsync(
        int roomId)
    {
        return await _context.Rooms
            .FirstOrDefaultAsync(
                x =>
                    x.RoomId == roomId);
    }


    // =========================================================
    // ADD ROOM
    // =========================================================
    public async Task<Room> AddAsync(
        Room room)
    {
        _context.Rooms.Add(room);

        await _context.SaveChangesAsync();

        return room;
    }


    // =========================================================
    // UPDATE ROOM
    // =========================================================
    public async Task UpdateAsync(
        Room room)
    {
        _context.Rooms.Update(room);

        await _context.SaveChangesAsync();
    }


    // =========================================================
    // GET OCCUPIED COUNT
    //
    // RoomAssigned = current correct status
    // Approved     = supports older existing records
    //                that already have AssignedRoomId
    // =========================================================
    public async Task<int> GetOccupiedCountAsync(
        int roomId)
    {
        return await _context.HostelApplications
            .CountAsync(x =>
                x.AssignedRoomId == roomId
                &&
                (
                    x.Status ==
                        HostelApplicationStatus.RoomAssigned
                    ||
                    x.Status ==
                        HostelApplicationStatus.Approved
                ));
    }


    // =========================================================
    // TOTAL ACTIVE CAPACITY OF HOSTEL
    // =========================================================
    public async Task<int>
        GetTotalActiveCapacityByHostelAsync(
            int hostelId)
    {
        return await _context.Rooms
            .Where(x =>
                x.HostelId == hostelId
                &&
                x.IsActive)
            .SumAsync(x =>
                x.Capacity);
    }


    // =========================================================
    // GET FILTERED ROOMS
    // =========================================================
    public async Task<List<Room>> GetFilteredAsync(
        int? hostelId,
        bool? availableOnly)
    {
        var query =
            _context.Rooms
                .AsNoTracking()
                .AsQueryable();


        if (hostelId.HasValue)
        {
            query = query.Where(
                x =>
                    x.HostelId ==
                    hostelId.Value);
        }


        var rooms =
            await query
                .OrderBy(x =>
                    x.RoomNumber)
                .ToListAsync();


        if (availableOnly != true)
        {
            return rooms;
        }


        var availableRooms =
            new List<Room>();


        foreach (var room in rooms)
        {
            if (!room.IsActive)
            {
                continue;
            }


            var occupied =
                await _context
                    .HostelApplications
                    .CountAsync(x =>
                        x.AssignedRoomId ==
                            room.RoomId
                        &&
                        (
                            x.Status ==
                                HostelApplicationStatus.RoomAssigned
                            ||
                            x.Status ==
                                HostelApplicationStatus.Approved
                        ));


            if (occupied < room.Capacity)
            {
                availableRooms.Add(room);
            }
        }


        return availableRooms;
    }


    // =========================================================
    // CHECK DUPLICATE ROOM NUMBER
    // =========================================================
    public async Task<bool> RoomNumberExistsAsync(
        int hostelId,
        string roomNumber,
        int? excludeRoomId = null)
    {
        var normalizedRoomNumber =
            roomNumber.Trim();


        var query =
            _context.Rooms
                .Where(x =>
                    x.HostelId ==
                        hostelId
                    &&
                    x.RoomNumber ==
                        normalizedRoomNumber);


        if (excludeRoomId.HasValue)
        {
            query = query.Where(
                x =>
                    x.RoomId !=
                    excludeRoomId.Value);
        }


        return await query.AnyAsync();
    }
}