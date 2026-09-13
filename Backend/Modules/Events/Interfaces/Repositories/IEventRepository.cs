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

    Task AddAsync(
        Event eventEntity);

    void Update(
        Event eventEntity);

    void Delete(
        Event eventEntity);

    Task SaveChangesAsync();
}