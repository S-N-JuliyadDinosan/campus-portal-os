using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Notifications.DTOs;
using CampusServicesPortal.Modules.Notifications.Entities;
using CampusServicesPortal.Modules.Notifications.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Notifications.Repositories;

public sealed class NotificationRepository
    : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Notification>>
        GetByStudentIdAsync(
            int studentId,
            NotificationFilterDto filter)
    {
        IQueryable<Notification> query =
            _context.Notifications
                .AsNoTracking()
                .Where(n => n.StudentId == studentId);

        if (filter.IsRead.HasValue)
        {
            query = query.Where(n =>
                n.IsRead == filter.IsRead.Value);
        }

        var page = filter.Page < 1
            ? 1
            : filter.Page;

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * 10)
            .Take(10)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notification>>
        GetUnreadByStudentIdAsync(
            int studentId)
    {
        return await _context.Notifications
            .Where(n =>
                n.StudentId == studentId &&
                !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Notification?>
        GetByIdAsync(
            int notificationId)
    {
        return await _context.Notifications
            .AsNoTracking()
            .FirstOrDefaultAsync(n =>
                n.NotificationId == notificationId);
    }

    public async Task<int>
        GetUnreadCountAsync(
            int studentId)
    {
        return await _context.Notifications
            .CountAsync(n =>
                n.StudentId == studentId &&
                !n.IsRead);
    }

    public async Task AddAsync(
        Notification notification)
    {
        await _context.Notifications
            .AddAsync(notification);
    }

    public void Update(
        Notification notification)
    {
        _context.Notifications
            .Update(notification);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}