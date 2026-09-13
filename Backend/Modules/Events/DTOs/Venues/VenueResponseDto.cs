using CampusServicesPortal.Modules.Events.Enums;

namespace CampusService.Modules.Events.DTOs.Venues;

public sealed class VenueResponseDto
{
    public int VenueId { get; set; }

    public string Name { get; set; } = string.Empty;

    public VenueType VenueType { get; set; }

    public string Location { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}