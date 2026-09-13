namespace CampusServicesPortal.Modules.Events.DTOs.EventSeats;

public sealed class SeatAvailabilityResponseDto
{
    public int EventSeatId { get; set; }

    public string SeatNumber { get; set; } = string.Empty;

    public string? SectionName { get; set; }

    public string? RowLabel { get; set; }

    public bool IsAvailable { get; set; }
}