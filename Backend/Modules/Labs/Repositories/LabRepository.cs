using CampusServicesPortal.Modules.Labs.Interfaces;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Labs.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Labs.Repositories;

public sealed class LabRepository : ILabRepository
{
    private readonly ApplicationDbContext _context;

    public LabRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Lab>> GetAllAsync()
    {
        return await _context.Labs
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Lab?> GetByIdAsync(int labId)
    {
        return await _context.Labs
            .FirstOrDefaultAsync(x => x.LabId == labId);
    }

    public async Task<Lab> AddAsync(Lab lab)
    {
        _context.Labs.Add(lab);

        await _context.SaveChangesAsync();

        return lab;
    }

    public async Task UpdateAsync(Lab lab)
    {
        _context.Labs.Update(lab);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Lab lab)
    {
        var bookingIds = await _context.LabBookings
            .Where(x => x.LabId == lab.LabId)
            .Select(x => x.LabBookingId)
            .ToListAsync();

        if (bookingIds.Count > 0)
        {
            var linkedFeePayments = await _context.FeePayments
                .Where(x => x.LabBookingId.HasValue &&
                            bookingIds.Contains(x.LabBookingId.Value))
                .ToListAsync();

            foreach (var feePayment in linkedFeePayments)
            {
                feePayment.LabBookingId = null;
            }

            _context.LabBookings.RemoveRange(
                await _context.LabBookings
                    .Where(x => x.LabId == lab.LabId)
                    .ToListAsync());
        }

        _context.LabSeats.RemoveRange(
            await _context.LabSeats
                .Where(x => x.LabId == lab.LabId)
                .ToListAsync());
        _context.LabTimeSlots.RemoveRange(
            await _context.LabTimeSlots
                .Where(x => x.LabId == lab.LabId)
                .ToListAsync());
        _context.Labs.Remove(lab);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> CodeExistsAsync(
        string code,
        int? excludeLabId = null)
    {
        var query = _context.Labs
            .Where(x => x.Code == code);

        if (excludeLabId.HasValue)
        {
            query = query.Where(
                x => x.LabId != excludeLabId.Value);
        }

        return await query.AnyAsync();
    }
}
