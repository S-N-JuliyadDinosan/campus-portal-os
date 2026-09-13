namespace CampusServicesPortal.Modules.Events.DTOs.EventSeats;

public sealed class CreateEventSeatDto
{
    public string SeatNumber { get; set; } = string.Empty;

    public string? SectionName { get; set; }

    public string? RowLabel { get; set; }
}