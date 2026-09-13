namespace CampusServicesPortal.Modules.Notifications.DTOs;

public sealed class NotificationResponseDto
{
    public int NotificationId { get; set; }

    public int StudentId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; }
}