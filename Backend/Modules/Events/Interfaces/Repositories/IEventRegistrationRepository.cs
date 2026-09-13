using CampusServicesPortal.Modules.Events.DTOs.EventRegistrations;
using CampusServicesPortal.Modules.Events.Entities;

namespace CampusServicesPortal.Modules.Events.Interfaces.Repositories;

public interface IEventRegistrationRepository
{
    Task<IEnumerable<EventRegistration>> GetAllAsync(
        RegistrationFilterDto filter);

    Task<IEnumerable<EventRegistration>> GetByStudentIdAsync(
        int studentId);

    Task<EventRegistration?> GetByIdAsync(
        int registrationId);

    Task<bool> ExistsAsync(
        int registrationId);

    Task<IEnumerable<EventRegistration>>
        GetActiveRegistrationsByStudentAsync(
            int studentId,
            int eventId);

    Task<EventRegistration?>
        GetActiveSeatRegistrationAsync(
            int eventSeatId);


    // =====================================================
    // ACTIVE EVENT REGISTRATION COUNT
    //
    // Counts:
    // - Confirmed
    // - Held and not expired
    //
    // Does NOT count:
    // - Cancelled
    // - Expired
    // =====================================================

    Task<int> GetActiveRegistrationCountAsync(
        int eventId);


    // =====================================================
    // EXPIRE OLD HELD REGISTRATIONS
    //
    // Converts:
    // Held + ExpiresAt <= current UTC time
    //
    // Into:
    // Expired
    //
    // Used by background service.
    // =====================================================

    Task<int> ExpireHeldRegistrationsAsync(
        DateTime utcNow,
        CancellationToken cancellationToken = default);


    // =====================================================
    // ADD
    // =====================================================

    Task AddAsync(
        EventRegistration registration);


    // =====================================================
    // UPDATE
    // =====================================================

    void Update(
        EventRegistration registration);


    // =====================================================
    // DELETE
    // =====================================================

    void Delete(
        EventRegistration registration);


    // =====================================================
    // SAVE
    // =====================================================

    Task SaveChangesAsync();
}