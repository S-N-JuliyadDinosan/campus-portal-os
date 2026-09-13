using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Events.DTOs.EventRegistrations;
using CampusServicesPortal.Modules.Events.Entities;
using CampusServicesPortal.Modules.Events.Enums;
using CampusServicesPortal.Modules.Events.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Events.Repositories;

public sealed class EventRegistrationRepository
    : IEventRegistrationRepository
{
    private readonly ApplicationDbContext _context;

    public EventRegistrationRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }


    // =====================================================
    // GET ALL REGISTRATIONS
    // ADMIN FILTER / PAGING
    // =====================================================

    public async Task<IEnumerable<EventRegistration>> GetAllAsync(
        RegistrationFilterDto filter)
    {
        IQueryable<EventRegistration> query =
            _context.EventRegistrations
                .AsNoTracking()
                .Include(r => r.Student)
                .Include(r => r.Event)
                .Include(r => r.EventSeat);

        if (filter.EventId.HasValue)
        {
            query = query.Where(r =>
                r.EventId == filter.EventId.Value);
        }

        if (filter.StudentId.HasValue)
        {
            query = query.Where(r =>
                r.StudentId == filter.StudentId.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(r =>
                r.Status == filter.Status.Value);
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
            .OrderByDescending(r => r.RegisteredAt)
            .Skip(
                (page - 1) *
                pageSize)
            .Take(pageSize)
            .ToListAsync();
    }


    // =====================================================
    // GET REGISTRATIONS BY STUDENT
    // =====================================================

    public async Task<IEnumerable<EventRegistration>>
        GetByStudentIdAsync(
            int studentId)
    {
        return await _context.EventRegistrations
            .AsNoTracking()
            .Include(r => r.Event)
                .ThenInclude(e => e.Venue)
            .Include(r => r.EventSeat)
            .Where(r =>
                r.StudentId == studentId)
            .OrderByDescending(r =>
                r.RegisteredAt)
            .ToListAsync();
    }


    // =====================================================
    // GET REGISTRATION BY ID
    // =====================================================

    public async Task<EventRegistration?> GetByIdAsync(
        int registrationId)
    {
        return await _context.EventRegistrations
            .AsNoTracking()
            .Include(r => r.Student)
            .Include(r => r.Event)
                .ThenInclude(e => e.Venue)
            .Include(r => r.EventSeat)
            .FirstOrDefaultAsync(r =>
                r.EventRegistrationId ==
                registrationId);
    }


    // =====================================================
    // CHECK REGISTRATION EXISTS
    // =====================================================

    public async Task<bool> ExistsAsync(
        int registrationId)
    {
        return await _context.EventRegistrations
            .AnyAsync(r =>
                r.EventRegistrationId ==
                registrationId);
    }


    // =====================================================
    // GET ACTIVE REGISTRATIONS FOR
    // ONE STUDENT + ONE EVENT
    //
    // Active:
    // - Confirmed
    // - Held and not expired
    // =====================================================

    public async Task<IEnumerable<EventRegistration>>
        GetActiveRegistrationsByStudentAsync(
            int studentId,
            int eventId)
    {
        var now =
            DateTime.UtcNow;

        return await _context.EventRegistrations
            .AsNoTracking()
            .Where(r =>
                r.StudentId == studentId
                &&
                r.EventId == eventId
                &&
                (
                    r.Status ==
                        EventRegistrationStatus.Confirmed
                    ||
                    (
                        r.Status ==
                            EventRegistrationStatus.Held
                        &&
                        r.ExpiresAt.HasValue
                        &&
                        r.ExpiresAt.Value > now
                    )
                ))
            .ToListAsync();
    }


    // =====================================================
    // CHECK WHETHER EVENT SEAT IS CURRENTLY OCCUPIED
    //
    // Occupied:
    // - Confirmed
    // - Held and not expired
    // =====================================================

    public async Task<EventRegistration?>
        GetActiveSeatRegistrationAsync(
            int eventSeatId)
    {
        var now =
            DateTime.UtcNow;

        return await _context.EventRegistrations
            .AsNoTracking()
            .FirstOrDefaultAsync(r =>
                r.EventSeatId == eventSeatId
                &&
                (
                    r.Status ==
                        EventRegistrationStatus.Confirmed
                    ||
                    (
                        r.Status ==
                            EventRegistrationStatus.Held
                        &&
                        r.ExpiresAt.HasValue
                        &&
                        r.ExpiresAt.Value > now
                    )
                ));
    }


    // =====================================================
    // GET ACTIVE REGISTRATION COUNT FOR EVENT
    //
    // Counts:
    // - Confirmed
    // - Held and not expired
    //
    // Does NOT count:
    // - Cancelled
    // - Expired
    // =====================================================

    public async Task<int> GetActiveRegistrationCountAsync(
        int eventId)
    {
        var now =
            DateTime.UtcNow;

        return await _context.EventRegistrations
            .CountAsync(r =>
                r.EventId == eventId
                &&
                (
                    r.Status ==
                        EventRegistrationStatus.Confirmed
                    ||
                    (
                        r.Status ==
                            EventRegistrationStatus.Held
                        &&
                        r.ExpiresAt.HasValue
                        &&
                        r.ExpiresAt.Value > now
                    )
                ));
    }


    // =====================================================
    // EXPIRE OLD HELD REGISTRATIONS
    //
    // Example:
    //
    // Status    = Held
    // ExpiresAt = 10:30
    // Current   = 10:31
    //
    // Result:
    // Status becomes Expired automatically.
    //
    // ExecuteUpdateAsync updates directly in SQL.
    // SaveChangesAsync is NOT required afterwards.
    // =====================================================

    public async Task<int> ExpireHeldRegistrationsAsync(
        DateTime utcNow,
        CancellationToken cancellationToken = default)
    {
        return await _context.EventRegistrations
            .Where(r =>
                r.Status ==
                    EventRegistrationStatus.Held
                &&
                r.ExpiresAt.HasValue
                &&
                r.ExpiresAt.Value <= utcNow)
            .ExecuteUpdateAsync(
                setters =>
                    setters.SetProperty(
                        r => r.Status,
                        EventRegistrationStatus.Expired),
                cancellationToken);
    }


    // =====================================================
    // ADD
    // =====================================================

    public async Task AddAsync(
        EventRegistration registration)
    {
        await _context.EventRegistrations
            .AddAsync(registration);
    }


    // =====================================================
    // UPDATE
    // =====================================================

    public void Update(
        EventRegistration registration)
    {
        _context.EventRegistrations
            .Update(registration);
    }


    // =====================================================
    // DELETE
    // =====================================================

    public void Delete(
        EventRegistration registration)
    {
        _context.EventRegistrations
            .Remove(registration);
    }


    // =====================================================
    // SAVE
    // =====================================================

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}