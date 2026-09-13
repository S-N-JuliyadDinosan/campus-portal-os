using CampusServicesPortal.Modules.Events.Entities;

namespace CampusServicesPortal.Modules.Events.Interfaces.Repositories;

public interface IVenueRepository
{
    Task<IEnumerable<Venue>> GetAllAsync();

    Task<Venue?> GetByIdAsync(
        int venueId);

    Task<bool> ExistsAsync(
        int venueId);

    Task<bool> IsVenueAvailableAsync(
        int venueId,
        DateTime from,
        DateTime to,
        int? excludeEventId = null);

    Task AddAsync(
        Venue venue);

    void Update(
        Venue venue);

    void Delete(
        Venue venue);

    Task SaveChangesAsync();
}