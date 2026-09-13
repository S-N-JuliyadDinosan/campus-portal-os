namespace CampusServicesPortal.Modules.Events.DTOs.EventSeats;

public sealed class UpdateEventSeatDto
{
    public string SeatNumber { get; set; } = string.Empty;

    public string? SectionName { get; set; }

    public string? RowLabel { get; set; }

    public bool IsActive { get; set; }
}