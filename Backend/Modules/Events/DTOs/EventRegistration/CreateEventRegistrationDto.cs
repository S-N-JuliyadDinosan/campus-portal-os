namespace CampusServicesPortal.Modules.Events.DTOs.EventRegistrations;

public sealed class CreateEventRegistrationDto
{
    public int EventId { get; set; }

    public int? EventSeatId { get; set; }
}