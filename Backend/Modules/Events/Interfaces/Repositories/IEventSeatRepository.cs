using CampusServicesPortal.Modules.Events.Entities;

namespace CampusServicesPortal.Modules.Events.Interfaces.Repositories;

public interface IEventSeatRepository
{
    Task<IEnumerable<EventSeat>> GetByEventIdAsync(int eventId);

    Task<EventSeat?> GetByIdAsync(int seatId);

    Task<bool> ExistsAsync(int seatId);

    Task AddAsync(EventSeat seat);

    void Update(EventSeat seat);

    void Delete(EventSeat seat);

    Task SaveChangesAsync();
}