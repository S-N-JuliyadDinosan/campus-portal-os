using CampusServicesPortal.Modules.Events.DTOs.Events;

namespace CampusServicesPortal.Modules.Events.Interfaces.Services;

public interface IEventService
{
    Task<IEnumerable<EventResponseDto>> GetAllAsync(EventFilterDto filter);

    Task<EventResponseDto?> GetByIdAsync(int eventId);

    Task<EventResponseDto> CreateAsync(CreateEventDto dto);

    Task UpdateAsync(int eventId, UpdateEventDto dto);

    Task DeleteAsync(int eventId);
}