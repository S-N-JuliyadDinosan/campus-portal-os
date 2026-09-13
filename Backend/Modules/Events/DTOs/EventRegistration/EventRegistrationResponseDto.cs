using CampusServicesPortal.Modules.Events.Enums;

namespace CampusServicesPortal.Modules.Events.DTOs.EventRegistrations;

public sealed class EventRegistrationResponseDto
{
    public int EventRegistrationId { get; set; }

    public int EventId { get; set; }

    public string EventTitle { get; set; } = string.Empty;

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public int? EventSeatId { get; set; }

    public string? SeatNumber { get; set; }

    public EventRegistrationStatus Status { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime RegisteredAt { get; set; }
}