using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Events.DTOs.Events;
using CampusServicesPortal.Modules.Events.Entities;
using CampusServicesPortal.Modules.Events.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Events.Repositories;

public sealed class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }


    // =====================================================
    // GET ALL EVENTS
    // FILTER + PAGINATION
    // =====================================================
    public async Task<IEnumerable<Event>> GetAllAsync(
        EventFilterDto filter)
    {
        IQueryable<Event> query =
            _context.Events
                .AsNoTracking()
                .Include(e => e.Venue)
                .Include(e => e.Registrations);


        if (filter.IsPublished.HasValue)
        {
            query = query.Where(e =>
                e.IsPublished ==
                filter.IsPublished.Value);
        }


        if (filter.From.HasValue)
        {
            query = query.Where(e =>
                e.StartAt >=
                filter.From.Value);
        }


        if (filter.To.HasValue)
        {
            query = query.Where(e =>
                e.EndAt <=
                filter.To.Value);
        }


        if (filter.VenueId.HasValue)
        {
            query = query.Where(e =>
                e.VenueId ==
                filter.VenueId.Value);
        }


        var page =
            filter.Page < 1
                ? 1
                : filter.Page;

        var pageSize =
            filter.PageSize < 1
                ? 10
                : filter.PageSize;


        return await query
            .OrderBy(e =>
                e.StartAt)
            .Skip(
                (page - 1) *
                pageSize)
            .Take(pageSize)
            .ToListAsync();
    }


    // =====================================================
    // GET EVENT BY ID
    // =====================================================
    public async Task<Event?> GetByIdAsync(
        int eventId)
    {
        return await _context.Events
            .AsNoTracking()
            .Include(e => e.Venue)
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e =>
                e.EventId == eventId);
    }


    // =====================================================
    // CHECK EVENT EXISTS
    // =====================================================
    public async Task<bool> ExistsAsync(
        int eventId)
    {
        return await _context.Events
            .AnyAsync(e =>
                e.EventId == eventId);
    }


    // =====================================================
    // GET MAX ACTIVE EVENT CAPACITY FOR VENUE
    //
    // Example:
    //
    // Venue Capacity = 100
    //
    // Event A Capacity = 80
    // Event B Capacity = 60
    //
    // Result = 80
    //
    // Therefore admin cannot reduce Venue Capacity
    // below 80 while Event A is active.
    // =====================================================
    public async Task<int>
        GetMaxActiveEventCapacityByVenueAsync(
            int venueId)
    {
        var maxCapacity =
            await _context.Events
                .AsNoTracking()
                .Where(e =>
                    e.VenueId == venueId &&
                    e.IsActive)
                .Select(e =>
                    (int?)e.Capacity)
                .MaxAsync();

        return maxCapacity ?? 0;
    }


    // =====================================================
    // CHECK WHETHER A VENUE IS STILL REFERENCED
    // =====================================================
    public async Task<bool> HasEventsForVenueAsync(
        int venueId)
    {
        return await _context.Events
            .AsNoTracking()
            .AnyAsync(eventEntity =>
                eventEntity.VenueId == venueId);
    }


    // =====================================================
    // ADD EVENT
    // =====================================================
    public async Task AddAsync(
        Event eventEntity)
    {
        await _context.Events
            .AddAsync(eventEntity);
    }


    // =====================================================
    // UPDATE EVENT
    // =====================================================
    public void Update(
        Event eventEntity)
    {
        _context.Events
            .Update(eventEntity);
    }


    // =====================================================
    // DELETE EVENT WITH ITS DEPENDENT RECORDS
    // =====================================================
    public async Task DeleteWithDependentsAsync(
        int eventId)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        // Registrations reference both the event and, optionally, a seat. They
        // must be deleted before the seats and event because those FKs restrict
        // deletion rather than cascading it.
        await _context.EventRegistrations
            .Where(registration => registration.EventId == eventId)
            .ExecuteDeleteAsync();

        await _context.EventSeats
            .Where(seat => seat.EventId == eventId)
            .ExecuteDeleteAsync();

        await _context.Events
            .Where(eventEntity => eventEntity.EventId == eventId)
            .ExecuteDeleteAsync();

        await transaction.CommitAsync();
    }


    // =====================================================
    // SAVE
    // =====================================================
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
