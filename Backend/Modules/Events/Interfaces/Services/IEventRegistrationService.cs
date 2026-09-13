using CampusServicesPortal.Modules.Events.DTOs.EventRegistrations;

namespace CampusServicesPortal.Modules.Events.Interfaces.Services;

public interface IEventRegistrationService
{
    Task<EventRegistrationResponseDto> RegisterAsync(
        int studentId,
        CreateEventRegistrationDto dto);

    Task ConfirmAsync(int registrationId);

    Task<IEnumerable<MyEventRegistrationDto>> GetMyRegistrationsAsync(
        int studentId);

    Task<EventRegistrationResponseDto?> GetByIdAsync(
        int registrationId);

    Task<IEnumerable<EventRegistrationResponseDto>> GetAllAsync(
        RegistrationFilterDto filter);

    Task CancelAsync(int registrationId);
}