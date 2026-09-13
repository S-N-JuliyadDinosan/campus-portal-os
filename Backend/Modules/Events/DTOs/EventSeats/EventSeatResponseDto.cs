namespace CampusServicesPortal.Modules.Events.DTOs.EventSeats;

public sealed class EventSeatResponseDto
{
    public int EventSeatId { get; set; }

    public int EventId { get; set; }

    public string SeatNumber { get; set; } = string.Empty;

    public string? SectionName { get; set; }

    public string? RowLabel { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}