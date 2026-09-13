using CampusServicesPortal.Modules.Events.Enums;

namespace CampusServicesPortal.Modules.Events.DTOs.EventRegistrations;

public sealed class MyEventRegistrationDto
{
    public int EventRegistrationId { get; set; }

    public string EventTitle { get; set; } = string.Empty;

    public DateTime StartAt { get; set; }

    public string VenueName { get; set; } = string.Empty;

    public EventRegistrationStatus Status { get; set; }

    public string? SeatNumber { get; set; }
}