using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Hostels.Entities;
using CampusServicesPortal.Modules.Hostels.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Hostels.Repositories;

public sealed class HostelRepository : IHostelRepository
{
    private readonly ApplicationDbContext _context;

    public HostelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Hostel>> GetAllAsync()
    {
        return await _context.Hostels
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Hostel?> GetByIdAsync(int hostelId)
    {
        return await _context.Hostels
            .FirstOrDefaultAsync(
                x => x.HostelId == hostelId);
    }

    public async Task<Hostel?> GetByNameAsync(string name)
    {
        return await _context.Hostels
            .FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<Hostel> AddAsync(Hostel hostel)
    {
        _context.Hostels.Add(hostel);

        await _context.SaveChangesAsync();

        return hostel;
    }

    public async Task UpdateAsync(Hostel hostel)
    {
        _context.Hostels.Update(hostel);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Hostel hostel)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var hostelId = hostel.HostelId;

        await _context.HostelApplications
            .Where(application =>
                application.PreferredHostelId == hostelId ||
                (application.AssignedRoomId.HasValue &&
                 _context.Rooms.Any(room =>
                     room.RoomId == application.AssignedRoomId.Value &&
                     room.HostelId == hostelId)))
            .ExecuteDeleteAsync();

        await _context.Rooms
            .Where(room => room.HostelId == hostelId)
            .ExecuteDeleteAsync();

        _context.Hostels.Remove(hostel);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();
    }

    public async Task<bool> NameExistsAsync(
    string name,
    int? excludeHostelId = null)
    {
        var query = _context.Hostels
            .Where(x => x.Name == name);

        if (excludeHostelId.HasValue)
        {
            query = query.Where(
                x => x.HostelId != excludeHostelId.Value);
        }

        return await query.AnyAsync();
    }
}
