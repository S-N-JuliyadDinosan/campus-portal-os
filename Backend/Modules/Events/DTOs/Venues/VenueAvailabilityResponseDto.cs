namespace CampusService.Modules.Events.DTOs.Venues;

public sealed class VenueAvailabilityResponseDto
{
    public int VenueId { get; set; }

    public string VenueName { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }

    public string? Message { get; set; }
}