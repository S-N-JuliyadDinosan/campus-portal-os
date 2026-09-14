using CampusServicesPortal.Modules.Events.DTOs.Events;
using CampusServicesPortal.Modules.Events.Entities;

namespace CampusServicesPortal.Modules.Events.Interfaces.Repositories;

public interface IEventRepository
{
    Task<IEnumerable<Event>> GetAllAsync(
        EventFilterDto filter);

    Task<Event?> GetByIdAsync(
        int eventId);

    Task<bool> ExistsAsync(
        int eventId);

    // =====================================================
    // GET HIGHEST ACTIVE EVENT CAPACITY FOR A VENUE
    //
    // Used when admin tries to reduce Venue.Capacity.
    // =====================================================
    Task<int> GetMaxActiveEventCapacityByVenueAsync(
        int venueId);

    Task<bool> HasEventsForVenueAsync(
        int venueId);

    Task AddAsync(
        Event eventEntity);

    void Update(
        Event eventEntity);

    // Removes the event and its event-owned records (seats and registrations).
    // The database relationships are restrictive, so these must be removed together.
    Task DeleteWithDependentsAsync(
        int eventId);

    Task SaveChangesAsync();
}
