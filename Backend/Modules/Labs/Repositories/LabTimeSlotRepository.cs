using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Labs.Entities;
using CampusServicesPortal.Modules.Labs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Labs.Repositories;

public sealed class LabTimeSlotRepository
    : ILabTimeSlotRepository
{
    private readonly ApplicationDbContext _context;

    public LabTimeSlotRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LabTimeSlot>> GetByLabIdAsync(
        int labId)
    {
        return await _context.LabTimeSlots
            .AsNoTracking()
            .Where(x => x.LabId == labId)
            .OrderBy(x => x.DayOfWeek)
            .ThenBy(x => x.StartTime)
            .ToListAsync();
    }

    public async Task<LabTimeSlot?> GetByIdAsync(
        int labTimeSlotId)
    {
        return await _context.LabTimeSlots
            .FirstOrDefaultAsync(
                x => x.LabTimeSlotId == labTimeSlotId);
    }

    public async Task<LabTimeSlot> AddAsync(
        LabTimeSlot timeSlot)
    {
        _context.LabTimeSlots.Add(timeSlot);

        await _context.SaveChangesAsync();

        return timeSlot;
    }

    public async Task UpdateAsync(
        LabTimeSlot timeSlot)
    {
        _context.LabTimeSlots.Update(timeSlot);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> TimeSlotExistsAsync(
        int labId,
        string dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        int? excludeTimeSlotId = null)
    {
        if (!Enum.TryParse<DayOfWeek>(
                dayOfWeek,
                true,
                out var parsedDay))
        {
            return false;
        }

        var start = TimeOnly.FromTimeSpan(startTime);
        var end = TimeOnly.FromTimeSpan(endTime);

        var query = _context.LabTimeSlots
            .Where(x =>
                x.LabId == labId &&
                x.DayOfWeek == parsedDay &&
                x.StartTime == start &&
                x.EndTime == end);

        if (excludeTimeSlotId.HasValue)
        {
            query = query.Where(x =>
                x.LabTimeSlotId != excludeTimeSlotId.Value);
        }

        return await query.AnyAsync();
    }
}