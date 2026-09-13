using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Events.Entities;
using CampusServicesPortal.Modules.Events.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Events.Repositories;

public sealed class VenueRepository : IVenueRepository
{
    private readonly ApplicationDbContext _context;

    public VenueRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }


    // =====================================================
    // GET ALL ACTIVE VENUES
    // =====================================================
    public async Task<IEnumerable<Venue>> GetAllAsync()
    {
        return await _context.Venues
            .AsNoTracking()
            .Where(v => v.IsActive)
            .OrderBy(v => v.Name)
            .ToListAsync();
    }


    // =====================================================
    // GET ACTIVE VENUE BY ID
    // =====================================================
    public async Task<Venue?> GetByIdAsync(
        int venueId)
    {
        return await _context.Venues
            .AsNoTracking()
            .FirstOrDefaultAsync(v =>
                v.VenueId == venueId &&
                v.IsActive);
    }


    // =====================================================
    // CHECK VENUE EXISTS
    // =====================================================
    public async Task<bool> ExistsAsync(
        int venueId)
    {
        return await _context.Venues
            .AnyAsync(v =>
                v.VenueId == venueId &&
                v.IsActive);
    }


    // =====================================================
    // CHECK VENUE AVAILABILITY
    //
    // CREATE:
    // excludeEventId = null
    //
    // UPDATE:
    // exclude current event ID so that the event
    // does not conflict with itself.
    // =====================================================
    public async Task<bool> IsVenueAvailableAsync(
        int venueId,
        DateTime from,
        DateTime to,
        int? excludeEventId = null)
    {
        var query =
            _context.Events
                .AsNoTracking()
                .Where(e =>
                    e.VenueId == venueId &&
                    e.IsActive &&
                    from < e.EndAt &&
                    to > e.StartAt);


        if (excludeEventId.HasValue)
        {
            query =
                query.Where(e =>
                    e.EventId !=
                    excludeEventId.Value);
        }


        var hasConflict =
            await query.AnyAsync();

        return !hasConflict;
    }


    // =====================================================
    // ADD
    // =====================================================
    public async Task AddAsync(
        Venue venue)
    {
        await _context.Venues
            .AddAsync(venue);
    }


    // =====================================================
    // UPDATE
    // =====================================================
    public void Update(
        Venue venue)
    {
        _context.Venues
            .Update(venue);
    }


    // =====================================================
    // DELETE
    // =====================================================
    public void Delete(
        Venue venue)
    {
        _context.Venues
            .Remove(venue);
    }


    // =====================================================
    // SAVE
    // =====================================================
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}