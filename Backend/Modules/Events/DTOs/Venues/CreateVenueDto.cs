using CampusServicesPortal.Modules.Events.Enums;

namespace CampusServicesPortal.Modules.Events.DTOs.Venues;

public sealed class CreateVenueDto
{
    public string Name { get; set; } = string.Empty;

    public VenueType VenueType { get; set; }

    public string Location { get; set; } = string.Empty;

    public int Capacity { get; set; }
}