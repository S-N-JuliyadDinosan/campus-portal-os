using CampusServicesPortal.Common.Entities;

namespace CampusServicesPortal.Modules.Events.Entities;

public sealed class EventSeat : AuditableEntity
{
    public int EventSeatId { get; set; }

    public int EventId { get; set; }

    public string SeatNumber { get; set; } = string.Empty;

    public string? SectionName { get; set; }

    public string? RowLabel { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Event Event { get; set; } = null!;

    public ICollection<EventRegistration> Registrations { get; set; } = [];
}