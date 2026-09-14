using System.Data;
using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Common.Security;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Hostels.DTOs;
using CampusServicesPortal.Modules.Hostels.Entities;
using CampusServicesPortal.Modules.Hostels.Interfaces;
using CampusServicesPortal.Modules.Notifications.DTOs;
using CampusServicesPortal.Modules.Notifications.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Hostels.Services;

public sealed class HostelApplicationService
    : IHostelApplicationService
{
    private readonly IHostelApplicationRepository
        _applicationRepository;

    private readonly IHostelRepository
        _hostelRepository;

    private readonly IRoomRepository
        _roomRepository;

    private readonly ICurrentUserService
        _currentUserService;

    private readonly INotificationService
        _notificationService;

    private readonly ApplicationDbContext
        _dbContext;


    public HostelApplicationService(
        IHostelApplicationRepository applicationRepository,
        IHostelRepository hostelRepository,
        IRoomRepository roomRepository,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ApplicationDbContext dbContext)
    {
        _applicationRepository =
            applicationRepository;

        _hostelRepository =
            hostelRepository;

        _roomRepository =
            roomRepository;

        _currentUserService =
            currentUserService;

        _notificationService =
            notificationService;

        _dbContext =
            dbContext;
    }


    // =========================================================
    // CREATE HOSTEL APPLICATION
    // Student only
    // =========================================================
    public async Task<HostelApplicationResponse> CreateAsync(
        CreateHostelApplicationRequest request)
    {
        var studentId =
            GetCurrentStudentId();


        var hostel =
            await _hostelRepository
                .GetByIdAsync(
                    request.PreferredHostelId);

        if (hostel == null)
        {
            throw new KeyNotFoundException(
                "Preferred hostel not found.");
        }


        if (!hostel.IsActive)
        {
            throw new InvalidOperationException(
                "Selected hostel is inactive.");
        }


        if (string.IsNullOrWhiteSpace(
                request.AcademicYear))
        {
            throw new ArgumentException(
                "Academic year is required.");
        }


        if (string.IsNullOrWhiteSpace(
                request.Semester))
        {
            throw new ArgumentException(
                "Semester is required.");
        }


        var academicYear =
            request.AcademicYear.Trim();

        var semester =
            request.Semester.Trim();


        var alreadyExists =
            await _applicationRepository
                .HasActiveApplicationAsync(
                    studentId,
                    academicYear,
                    semester);

        if (alreadyExists)
        {
            throw new InvalidOperationException(
                "You already have an active hostel application for this academic year and semester.");
        }


        var application =
            new HostelApplication
            {
                // IMPORTANT:
                // Ignore request.StudentId.
                // StudentId comes from JWT.
                StudentId =
                    studentId,

                PreferredHostelId =
                    request.PreferredHostelId,

                AcademicYear =
                    academicYear,

                Semester =
                    semester,

                SpecialRequirements =
                    string.IsNullOrWhiteSpace(
                        request.SpecialRequirements)
                        ? null
                        : request
                            .SpecialRequirements
                            .Trim(),

                Status =
                    HostelApplicationStatus.Pending
            };


        application =
            await _applicationRepository
                .AddAsync(application);


        var created =
            await _applicationRepository
                .GetByIdAsync(
                    application
                        .HostelApplicationId);


        return MapToResponse(
            created ?? application);
    }


    // =========================================================
    // GET APPLICATION BY ID
    // =========================================================
    public async Task<HostelApplicationResponse?>
        GetByIdAsync(
            int hostelApplicationId)
    {
        var application =
            await _applicationRepository
                .GetByIdAsync(
                    hostelApplicationId);


        return application == null
            ? null
            : MapToResponse(application);
    }


    // =========================================================
    // GET STUDENT APPLICATIONS
    // =========================================================
    public async Task<List<HostelApplicationResponse>>
        GetByStudentIdAsync(
            int studentId)
    {
        var applications =
            await _applicationRepository
                .GetByStudentIdAsync(
                    studentId);


        return applications
            .Select(MapToResponse)
            .ToList();
    }


    // =========================================================
    // ADMIN - GET ALL APPLICATIONS
    // =========================================================
    public async Task<List<HostelApplicationResponse>>
        GetAllAsync(
            int? hostelId = null,
            string? status = null,
            string? academicYear = null,
            string? semester = null,
            int page = 1)
    {
        if (page < 1)
        {
            throw new ArgumentException(
                "Page must be greater than zero.");
        }


        var applications =
            await _applicationRepository
                .GetAllAsync(
                    hostelId,
                    status,
                    academicYear,
                    semester,
                    page);


        return applications
            .Select(MapToResponse)
            .ToList();
    }


    // =========================================================
    // ADMIN - APPROVE / REJECT
    // =========================================================
    public async Task<bool> UpdateStatusAsync(
        int hostelApplicationId,
        UpdateHostelApplicationStatusRequest request,
        int? reviewedByUserId = null)
    {
        var adminUserId =
            GetCurrentAdminUserId(
                reviewedByUserId);


        var application =
            await _applicationRepository
                .GetByIdAsync(
                    hostelApplicationId);

        if (application == null)
        {
            return false;
        }


        if (application.Status !=
            HostelApplicationStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only a pending hostel application can be approved or rejected.");
        }


        if (string.IsNullOrWhiteSpace(
                request.Status))
        {
            throw new ArgumentException(
                "Application status is required.");
        }


        if (!Enum.TryParse<
                HostelApplicationStatus>(
                request.Status.Trim(),
                true,
                out var newStatus))
        {
            throw new ArgumentException(
                "Invalid application status.");
        }


        // Admin status endpoint is ONLY
        // Pending -> Approved / Rejected
        if (newStatus !=
                HostelApplicationStatus.Approved
            &&
            newStatus !=
                HostelApplicationStatus.Rejected)
        {
            throw new InvalidOperationException(
                "Application status can only be changed to Approved or Rejected from this endpoint.");
        }


        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync();

        try
        {
            application.Status =
                newStatus;

            application.ReviewedByUserId =
                adminUserId;

            application.ReviewedAt =
                DateTime.UtcNow;


            if (newStatus ==
                HostelApplicationStatus.Rejected)
            {
                application.AssignedRoomId =
                    null;
            }


            await _applicationRepository
                .UpdateAsync(application);


            if (newStatus ==
                HostelApplicationStatus.Approved)
            {
                await CreateNotificationAsync(
                    application.StudentId,
                    "HostelApproved",
                    "Hostel application approved",
                    "Your hostel application has been approved.");
            }
            else
            {
                await CreateNotificationAsync(
                    application.StudentId,
                    "HostelRejected",
                    "Hostel application rejected",
                    "Your hostel application has been rejected.");
            }


            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }


    // =========================================================
    // ADMIN - ASSIGN ROOM
    // =========================================================
    public async Task<bool> AssignRoomAsync(
        int hostelApplicationId,
        AssignRoomRequest request,
        int? reviewedByUserId = null)
    {
        var adminUserId =
            GetCurrentAdminUserId(
                reviewedByUserId);


        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    IsolationLevel.Serializable);

        try
        {
            var application =
                await _applicationRepository
                    .GetByIdAsync(
                        hostelApplicationId);

            if (application == null)
            {
                await transaction.RollbackAsync();

                return false;
            }


            // BRD:
            // application must be approved FIRST
            if (application.Status !=
                HostelApplicationStatus.Approved)
            {
                throw new InvalidOperationException(
                    "The hostel application must be approved before a room can be assigned.");
            }


            if (application.AssignedRoomId
                .HasValue)
            {
                throw new InvalidOperationException(
                    "A room is already assigned to this application.");
            }


            var room =
                await _roomRepository
                    .GetByIdAsync(
                        request.RoomId);

            if (room == null)
            {
                throw new KeyNotFoundException(
                    "Room not found.");
            }


            if (!room.IsActive)
            {
                throw new InvalidOperationException(
                    "Selected room is inactive.");
            }


            if (room.HostelId !=
                application.PreferredHostelId)
            {
                throw new InvalidOperationException(
                    "Selected room does not belong to the preferred hostel.");
            }


            // Use corrected HostelApplicationRepository
            // occupancy calculation.
            var occupied =
                await _applicationRepository
                    .GetAssignedCountAsync(
                        room.RoomId);


            if (occupied >=
                room.Capacity)
            {
                throw new InvalidOperationException(
                    "Selected room is already full.");
            }


            application.AssignedRoomId =
                room.RoomId;

            application.Status =
                HostelApplicationStatus.RoomAssigned;

            application.ReviewedByUserId =
                adminUserId;

            application.ReviewedAt =
                DateTime.UtcNow;


            await _applicationRepository
                .UpdateAsync(application);


            await CreateNotificationAsync(
                application.StudentId,
                "HostelRoomAssigned",
                "Hostel room assigned",
                $"Room {room.RoomNumber} has been assigned to your hostel application.");


            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }


    // =========================================================
    // ADMIN - UNASSIGN ROOM
    // =========================================================
    public async Task<bool> UnassignRoomAsync(
        int hostelApplicationId,
        int? reviewedByUserId = null)
    {
        var adminUserId =
            GetCurrentAdminUserId(
                reviewedByUserId);


        var application =
            await _applicationRepository
                .GetByIdAsync(
                    hostelApplicationId);

        if (application == null)
        {
            return false;
        }


        if (application.Status !=
            HostelApplicationStatus.RoomAssigned)
        {
            throw new InvalidOperationException(
                "This hostel application does not currently have an assigned room.");
        }


        if (!application.AssignedRoomId
            .HasValue)
        {
            throw new InvalidOperationException(
                "No room is currently assigned to this application.");
        }


        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync();

        try
        {
            application.AssignedRoomId =
                null;

            // After room removal,
            // application remains approved.
            application.Status =
                HostelApplicationStatus.Approved;

            application.ReviewedByUserId =
                adminUserId;

            application.ReviewedAt =
                DateTime.UtcNow;


            await _applicationRepository
                .UpdateAsync(application);


            await CreateNotificationAsync(
                application.StudentId,
                "HostelRoomUnassigned",
                "Hostel room assignment updated",
                "Your previously assigned hostel room has been removed. Your application remains approved.");


            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }


    // =========================================================
    // STUDENT - CANCEL APPLICATION
    // =========================================================
    public async Task<bool> CancelAsync(
        int hostelApplicationId)
    {
        var currentStudentId =
            GetCurrentStudentId();


        var application =
            await _applicationRepository
                .GetByIdAsync(
                    hostelApplicationId);

        if (application == null)
        {
            return false;
        }


        // Student cannot cancel another
        // student's application.
        if (application.StudentId !=
            currentStudentId)
        {
            throw new UnauthorizedAccessException(
                "You cannot cancel another student's hostel application.");
        }


        if (application.Status !=
            HostelApplicationStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only a pending hostel application can be cancelled.");
        }


        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync();

        try
        {
            application.Status =
                HostelApplicationStatus.Cancelled;

            application.AssignedRoomId =
                null;


            await _applicationRepository
                .UpdateAsync(application);


            await CreateNotificationAsync(
                application.StudentId,
                "HostelCancelled",
                "Hostel application cancelled",
                "Your hostel application has been cancelled.");


            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }


    // =========================================================
    // NOTIFICATION HELPER
    // =========================================================
    private async Task CreateNotificationAsync(
        int studentId,
        string type,
        string title,
        string message)
    {
        await _notificationService
            .CreateAsync(
                new CreateNotificationDto
                {
                    StudentId =
                        studentId,

                    Type =
                        type,

                    Title =
                        title,

                    Message =
                        message
                });
    }


    // =========================================================
    // GET CURRENT STUDENT FROM JWT
    // =========================================================
    private int GetCurrentStudentId()
    {
        if (!_currentUserService.StudentId
            .HasValue)
        {
            throw new UnauthorizedAccessException(
                "The current account is not associated with a student.");
        }


        return _currentUserService
            .StudentId
            .Value;
    }


    // =========================================================
    // GET CURRENT ADMIN FROM JWT
    // =========================================================
    private int GetCurrentAdminUserId(
        int? reviewedByUserId)
    {
        if (!string.Equals(
                _currentUserService.Role,
                "Admin",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException(
                "Administrator access is required.");
        }


        var userId =
            reviewedByUserId ??
            _currentUserService.UserId;


        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "Administrator identity was not found.");
        }


        return userId.Value;
    }


    // =========================================================
    // MAP ENTITY -> RESPONSE
    // =========================================================
    private static HostelApplicationResponse
        MapToResponse(
            HostelApplication application)
    {
        return new HostelApplicationResponse
        {
            HostelApplicationId =
                application.HostelApplicationId,

            StudentId =
                application.StudentId,

            StudentName =
                application.Student?.FullName
                ?? string.Empty,

            StudentIndexNumber =
                application.Student?.IndexNumber
                ?? string.Empty,

            PreferredHostelId =
                application.PreferredHostelId,

            PreferredHostelName =
                application.PreferredHostel?.Name,

            AssignedRoomId =
                application.AssignedRoomId,

            AssignedRoomNumber =
                application.AssignedRoom
                    ?.RoomNumber,

            ReviewedByUserId =
                application.ReviewedByUserId,

            Status =
                application.Status.ToString(),

            AcademicYear =
                application.AcademicYear,

            Semester =
                application.Semester,

            RequestedAt =
                application.CreatedAt,

            ReviewedAt =
                application.ReviewedAt
        };
    }
}
