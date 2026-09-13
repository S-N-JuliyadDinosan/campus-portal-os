using CampusServicesPortal.Common.Entities;
using CampusServicesPortal.Modules.Identity.Entities;

namespace CampusServicesPortal.Modules.Events.Entities;

public sealed class Event : AuditableEntity
{
    public int EventId { get; set; }
    public int VenueId { get; set; }
    public int CreatedByUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public int Capacity { get; set; }
    public bool UsesReservedSeating { get; set; }
    public bool IsPublished { get; set; }
    public bool IsActive { get; set; } = true;

    public Venue Venue { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
    public ICollection<EventSeat> Seats { get; set; } = [];
    public ICollection<EventRegistration> Registrations { get; set; } = [];
}
