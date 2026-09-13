using CampusServicesPortal.Modules.Notifications.DTOs;
using CampusServicesPortal.Modules.Notifications.Entities;

namespace CampusServicesPortal.Modules.Notifications.Interfaces.Repositories;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetByStudentIdAsync(
        int studentId,
        NotificationFilterDto filter);

    Task<IEnumerable<Notification>> GetUnreadByStudentIdAsync(
        int studentId);

    Task<Notification?> GetByIdAsync(
        int notificationId);

    Task<int> GetUnreadCountAsync(
        int studentId);

    Task AddAsync(
        Notification notification);

    void Update(
        Notification notification);

    Task SaveChangesAsync();
}