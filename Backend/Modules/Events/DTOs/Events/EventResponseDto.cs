using CampusServicesPortal.Modules.Events.Enums;

namespace CampusServicesPortal.Modules.Events.DTOs.Events;

public sealed class EventResponseDto
{
    public int EventId { get; set; }

    public int VenueId { get; set; }

    public string VenueName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    public int Capacity { get; set; }

    public int RegisteredCount { get; set; }

    public int AvailableSeats { get; set; }

    public bool UsesReservedSeating { get; set; }

    public bool IsPublished { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}