using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Labs.Entities;
using CampusServicesPortal.Modules.Labs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Labs.Repositories;

public sealed class LabSeatRepository : ILabSeatRepository
{
    private readonly ApplicationDbContext _context;

    public LabSeatRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LabSeat>> GetByLabIdAsync(
        int labId)
    {
        return await _context.LabSeats
            .AsNoTracking()
            .Where(x => x.LabId == labId)
            .OrderBy(x => x.SeatNumber)
            .ToListAsync();
    }

    public async Task<LabSeat?> GetByIdAsync(
        int labSeatId)
    {
        return await _context.LabSeats
            .FirstOrDefaultAsync(
                x => x.LabSeatId == labSeatId);
    }

    public async Task<LabSeat> AddAsync(
        LabSeat labSeat)
    {
        _context.LabSeats.Add(labSeat);

        await _context.SaveChangesAsync();

        return labSeat;
    }

    public async Task UpdateAsync(
        LabSeat labSeat)
    {
        _context.LabSeats.Update(labSeat);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> SeatNumberExistsAsync(
        int labId,
        string seatNumber,
        int? excludeLabSeatId = null)
    {
        var query = _context.LabSeats
            .Where(x =>
                x.LabId == labId &&
                x.SeatNumber == seatNumber);

        if (excludeLabSeatId.HasValue)
        {
            query = query.Where(x =>
                x.LabSeatId != excludeLabSeatId.Value);
        }

        return await query.AnyAsync();
    }
}