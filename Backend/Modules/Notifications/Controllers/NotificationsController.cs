using CampusServicesPortal.Common.Security;
using CampusServicesPortal.Modules.Notifications.DTOs;
using CampusServicesPortal.Modules.Notifications.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Modules.Notifications.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize(Roles = "Student")]
public sealed class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;

    public NotificationsController(
        INotificationService notificationService,
        ICurrentUserService currentUserService)
    {
        _notificationService = notificationService;
        _currentUserService = currentUserService;
    }

    // GET: api/notifications/me?isRead=false&page=1
    [HttpGet("me")]
    public async Task<
        ActionResult<IEnumerable<NotificationResponseDto>>>
        GetMyNotifications(
            [FromQuery] NotificationFilterDto filter)
    {
        var studentId = GetCurrentStudentId();

        var notifications =
            await _notificationService
                .GetMyNotificationsAsync(
                    studentId,
                    filter);

        return Ok(notifications);
    }

    // BRD: GET /api/notifications/student/{studentId}
    [HttpGet("student/{studentId:int}")]
    public async Task<ActionResult<IEnumerable<NotificationResponseDto>>>
        GetStudentNotifications(
            int studentId,
            [FromQuery] NotificationFilterDto filter)
    {
        var currentStudentId = GetCurrentStudentId();
        if (studentId != currentStudentId)
        {
            return Forbid();
        }

        var notifications = await _notificationService
            .GetMyNotificationsAsync(studentId, filter);

        return Ok(notifications);
    }

    // GET: api/notifications/me/unread-count
    [HttpGet("me/unread-count")]
    public async Task<ActionResult<int>>
        GetUnreadCount()
    {
        var studentId = GetCurrentStudentId();

        var count =
            await _notificationService
                .GetUnreadCountAsync(studentId);

        return Ok(count);
    }

    // PUT: api/notifications/10/read
    [HttpPut("{notificationId:int}/read")]
    public async Task<IActionResult> MarkAsRead(
        int notificationId)
    {
        var studentId = GetCurrentStudentId();

        await _notificationService
            .MarkAsReadAsync(
                notificationId,
                studentId);

        return NoContent();
    }

    // PUT: api/notifications/me/read-all
    [HttpPut("me/read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var studentId = GetCurrentStudentId();

        await _notificationService
            .MarkAllAsReadAsync(studentId);

        return NoContent();
    }

    private int GetCurrentStudentId()
    {
        if (!_currentUserService.StudentId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "The current user is not associated with a student.");
        }

        return _currentUserService.StudentId.Value;
    }
}