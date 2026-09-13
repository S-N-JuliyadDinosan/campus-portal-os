namespace CampusServicesPortal.Modules.Events.DTOs.Events;

public sealed class EventFilterDto
{
    public bool? IsPublished { get; set; }

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    public int? VenueId { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}