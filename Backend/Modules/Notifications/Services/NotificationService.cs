using CampusServicesPortal.Modules.Notifications.DTOs;
using CampusServicesPortal.Modules.Notifications.Entities;
using CampusServicesPortal.Modules.Notifications.Interfaces.Repositories;
using CampusServicesPortal.Modules.Notifications.Interfaces.Services;

namespace CampusServicesPortal.Modules.Notifications.Services;

public sealed class NotificationService
    : INotificationService
{
    private readonly INotificationRepository
        _notificationRepository;

    public NotificationService(
        INotificationRepository notificationRepository)
    {
        _notificationRepository =
            notificationRepository;
    }

    public async Task<IEnumerable<NotificationResponseDto>>
        GetMyNotificationsAsync(
            int studentId,
            NotificationFilterDto filter)
    {
        var notifications =
            await _notificationRepository
                .GetByStudentIdAsync(
                    studentId,
                    filter);

        return notifications
            .Select(MapToResponseDto);
    }

    public async Task<int>
        GetUnreadCountAsync(
            int studentId)
    {
        return await _notificationRepository
            .GetUnreadCountAsync(studentId);
    }

    public async Task MarkAsReadAsync(
        int notificationId,
        int studentId)
    {
        var notification =
            await _notificationRepository
                .GetByIdAsync(notificationId);

        if (notification is null)
        {
            throw new KeyNotFoundException(
                $"Notification with ID {notificationId} was not found.");
        }

        if (notification.StudentId != studentId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to modify this notification.");
        }

        if (notification.IsRead)
        {
            return;
        }

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;

        _notificationRepository.Update(notification);

        await _notificationRepository
            .SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(
        int studentId)
    {
        var notifications =
            await _notificationRepository
                .GetUnreadByStudentIdAsync(studentId);

        if (!notifications.Any())
        {
            return;
        }

        var readAt = DateTime.UtcNow;

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = readAt;

            _notificationRepository.Update(
                notification);
        }

        await _notificationRepository
            .SaveChangesAsync();
    }

    public async Task<NotificationResponseDto>
        CreateAsync(
            CreateNotificationDto dto)
    {
        var notification = new Notification
        {
            StudentId = dto.StudentId,
            Type = dto.Type,
            Title = dto.Title,
            Message = dto.Message,
            IsRead = false,
            ReadAt = null
        };

        await _notificationRepository
            .AddAsync(notification);

        await _notificationRepository
            .SaveChangesAsync();

        var created =
            await _notificationRepository
                .GetByIdAsync(
                    notification.NotificationId);

        if (created is null)
        {
            throw new InvalidOperationException(
                "Notification was created but could not be retrieved.");
        }

        return MapToResponseDto(created);
    }

    private static NotificationResponseDto
        MapToResponseDto(
            Notification notification)
    {
        return new NotificationResponseDto
        {
            NotificationId =
                notification.NotificationId,

            StudentId =
                notification.StudentId,

            Type =
                notification.Type,

            Title =
                notification.Title,

            Message =
                notification.Message,

            IsRead =
                notification.IsRead,

            CreatedAt =
                notification.CreatedAt,

            ReadAt =
                notification.ReadAt
        };
    }
}