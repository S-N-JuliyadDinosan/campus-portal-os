namespace CampusServicesPortal.Modules.Notifications.DTOs;

public sealed class CreateNotificationDto
{
    public int StudentId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}