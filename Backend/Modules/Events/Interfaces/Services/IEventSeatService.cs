using CampusServicesPortal.Modules.Events.DTOs.EventSeats;

namespace CampusServicesPortal.Modules.Events.Interfaces.Services;

public interface IEventSeatService
{
    Task<IEnumerable<EventSeatResponseDto>> GetByEventIdAsync(int eventId);

    Task<IEnumerable<SeatAvailabilityResponseDto>> GetAvailabilityAsync(int eventId);

    Task<EventSeatResponseDto> CreateAsync(int eventId, CreateEventSeatDto dto);

    Task UpdateAsync(int seatId, UpdateEventSeatDto dto);

    Task DeleteAsync(int seatId);
}