using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Events.Entities;
using CampusServicesPortal.Modules.Events.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Events.Repositories;

public sealed class EventSeatRepository : IEventSeatRepository
{
    private readonly ApplicationDbContext _context;

    public EventSeatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EventSeat>> GetByEventIdAsync(int eventId)
    {
        return await _context.EventSeats
            .AsNoTracking()
            .Where(s => s.EventId == eventId && s.IsActive)
            .OrderBy(s => s.SeatNumber)
            .ToListAsync();
    }

    public async Task<EventSeat?> GetByIdAsync(int seatId)
    {
        return await _context.EventSeats
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.EventSeatId == seatId);
    }

    public async Task<bool> ExistsAsync(int seatId)
    {
        return await _context.EventSeats
            .AnyAsync(s => s.EventSeatId == seatId);
    }

    public async Task AddAsync(EventSeat seat)
    {
        await _context.EventSeats.AddAsync(seat);
    }

    public void Update(EventSeat seat)
    {
        _context.EventSeats.Update(seat);
    }

    public void Delete(EventSeat seat)
    {
        _context.EventSeats.Remove(seat);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}