using CampusServicesPortal.Modules.Events.Enums;

namespace CampusServicesPortal.Modules.Events.DTOs.EventRegistrations;

public sealed class RegistrationFilterDto
{
    public int? EventId { get; set; }

    public int? StudentId { get; set; }

    public EventRegistrationStatus? Status { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}