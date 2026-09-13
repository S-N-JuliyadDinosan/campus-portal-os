namespace CampusServicesPortal.Modules.Notifications.DTOs;

public sealed class NotificationFilterDto
{
    public bool? IsRead { get; set; }

    public int Page { get; set; } = 1;
}