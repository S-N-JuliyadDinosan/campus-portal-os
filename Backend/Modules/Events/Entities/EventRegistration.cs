using CampusServicesPortal.Common.Entities;
using CampusServicesPortal.Modules.Events.Enums;
using CampusServicesPortal.Modules.Students.Entities;

namespace CampusServicesPortal.Modules.Events.Entities;

public sealed class EventRegistration : AuditableEntity
{
    public int EventRegistrationId { get; set; }

    public int EventId { get; set; }

    public int StudentId { get; set; }

    public int? EventSeatId { get; set; }

    public EventRegistrationStatus Status { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Event Event { get; set; } = null!;

    public Student Student { get; set; } = null!;

    public EventSeat? EventSeat { get; set; }
}