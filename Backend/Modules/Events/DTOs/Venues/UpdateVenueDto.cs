using CampusServicesPortal.Modules.Events.Enums;

namespace CampusService.Modules.Events.DTOs.Venues;

public sealed class UpdateVenueDto
{
    public string Name { get; set; } = string.Empty;

    public VenueType VenueType { get; set; }

    public string Location { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public bool IsActive { get; set; }
}