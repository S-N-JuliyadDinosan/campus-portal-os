using CampusServicesPortal.Modules.Notifications.DTOs;

namespace CampusServicesPortal.Modules.Notifications.Interfaces.Services;

public interface INotificationService
{
    Task<IEnumerable<NotificationResponseDto>>
        GetMyNotificationsAsync(
            int studentId,
            NotificationFilterDto filter);

    Task<int> GetUnreadCountAsync(
        int studentId);

    Task MarkAsReadAsync(
        int notificationId,
        int studentId);

    Task MarkAllAsReadAsync(
        int studentId);

    Task<NotificationResponseDto>
        CreateAsync(
            CreateNotificationDto dto);
}