using CampusServicesPortal.Common.Entities;
using CampusServicesPortal.Modules.Students.Entities;

namespace CampusServicesPortal.Modules.Notifications.Entities;

public sealed class Notification : AuditableEntity
{
    public int NotificationId { get; set; }

    public int StudentId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    // Navigation
    public Student Student { get; set; } = null!;
}