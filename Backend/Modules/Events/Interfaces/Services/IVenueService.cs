using CampusService.Modules.Events.DTOs.Venues;
using CampusServicesPortal.Modules.Events.DTOs.Venues;

namespace CampusServicesPortal.Modules.Events.Interfaces.Services;

public interface IVenueService
{
    Task<IEnumerable<VenueResponseDto>> GetAllAsync();

    Task<VenueResponseDto?> GetByIdAsync(int venueId);

    Task<VenueAvailabilityResponseDto> CheckAvailabilityAsync(
        int venueId,
        DateTime from,
        DateTime to);

    Task<VenueResponseDto> CreateAsync(CreateVenueDto dto);

    Task UpdateAsync(int venueId, UpdateVenueDto dto);

    Task DeleteAsync(int venueId);
}