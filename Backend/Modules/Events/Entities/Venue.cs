using CampusServicesPortal.Common.Entities;
using CampusServicesPortal.Modules.Events.Enums;

namespace CampusServicesPortal.Modules.Events.Entities;

public sealed class Venue : AuditableEntity
{
    public int VenueId { get; set; }

    public string Name { get; set; } = string.Empty;

    public VenueType VenueType { get; set; }

    public string Location { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Event> Events { get; set; } = [];
}