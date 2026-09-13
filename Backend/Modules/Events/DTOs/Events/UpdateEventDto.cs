namespace CampusServicesPortal.Modules.Events.DTOs.Events;

public sealed class UpdateEventDto
{
    public int VenueId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    public int Capacity { get; set; }

    public bool UsesReservedSeating { get; set; }

    public bool IsPublished { get; set; }

    public bool IsActive { get; set; }
}