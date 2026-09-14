using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Common.Security;
using CampusServicesPortal.Modules.Complaints.DTOs;
using CampusServicesPortal.Modules.Complaints.Entities;
using CampusServicesPortal.Modules.Complaints.Interfaces.Repositories;
using CampusServicesPortal.Modules.Complaints.Interfaces.Services;
using CampusServicesPortal.Modules.Notifications.DTOs;
using CampusServicesPortal.Modules.Notifications.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Complaints.Services;

public sealed class ComplaintService(
    IComplaintRepository repository,
    IComplaintCategoryRepository categoryRepository,
    ICurrentUserService currentUser,
    INotificationService notificationService)
    : IComplaintService
{
    public async Task<ComplaintResponseDto> CreateAsync(
        CreateComplaintDto dto)
    {
        var studentId = currentUser.StudentId;

        if (!studentId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "Authenticated student is required.");
        }

        var category =
            await categoryRepository.GetByIdAsync(
                dto.ComplaintCategoryId);

        if (category is null || !category.IsActive)
        {
            throw new InvalidOperationException(
                "Invalid or inactive complaint category.");
        }

        var complaint = new Complaint
        {
            StudentId = studentId.Value,
            ComplaintCategoryId = dto.ComplaintCategoryId,
            Description = dto.Description.Trim(),
            IsAnonymous = dto.IsAnonymous,
            Status = ComplaintStatus.Pending
        };

        await repository.AddAsync(complaint);
        await repository.SaveChangesAsync();

        var created =
            await repository.GetByIdAsync(
                complaint.ComplaintId);

        return MapToResponse(created!);
    }

    public async Task<List<ComplaintResponseDto>>
        GetMyComplaintsAsync()
    {
        var studentId = currentUser.StudentId;

        if (!studentId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "Authenticated student is required.");
        }

        var complaints =
            await repository.GetStudentComplaintsAsync(
                studentId.Value);

        return complaints
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<ComplaintResponseDto?>
        GetByIdAsync(int id)
    {
        var complaint =
            await repository.GetByIdAsync(id);

        if (complaint is null)
        {
            return null;
        }

        if (string.Equals(currentUser.Role, "Student", StringComparison.OrdinalIgnoreCase))
        {
            if (!currentUser.StudentId.HasValue ||
                complaint.StudentId != currentUser.StudentId.Value)
            {
                throw new UnauthorizedAccessException(
                    "You can only view your own complaints.");
            }
        }

        return MapToResponse(complaint);
    }

    public async Task<List<ComplaintResponseDto>>
        GetAllAsync(ComplaintFilterDto filter)
    {
        var query = repository.Query();

        if (filter.Status.HasValue)
        {
            query = query.Where(
                x => x.Status == filter.Status.Value);
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(
                x => x.ComplaintCategoryId
                    == filter.CategoryId.Value);
        }

        if (filter.StudentId.HasValue)
        {
            query = query.Where(
                x => x.StudentId
                    == filter.StudentId.Value);
        }

        var complaints = await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return complaints
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<bool> UpdateStatusAsync(
        int id,
        UpdateComplaintStatusDto dto)
    {
        var complaint =
            await repository.GetByIdAsync(id);

        if (complaint is null)
        {
            return false;
        }

        complaint.Status = dto.Status;

        if (dto.Status == ComplaintStatus.Resolved)
        {
            complaint.ResolutionNote =
                dto.ResolutionNote?.Trim();

            complaint.ResolvedAt =
                DateTime.UtcNow;
        }
        else
        {
            complaint.ResolutionNote = null;
            complaint.ResolvedAt = null;
        }

        complaint.StatusChangedByUserId =
            currentUser.UserId;

        await repository.UpdateAsync(complaint);
        await repository.SaveChangesAsync();

        await notificationService.CreateAsync(
            new CreateNotificationDto
            {
                StudentId = complaint.StudentId,
                Type = "ComplaintUpdated",
                Title = "Complaint status updated",
                Message = dto.Status == ComplaintStatus.Resolved
                    ? "Your complaint has been resolved."
                    : $"Your complaint status is now {dto.Status}."
            });

        return true;
    }

    private static ComplaintResponseDto MapToResponse(
        Complaint complaint)
    {
        var response = new ComplaintResponseDto
        {
            ComplaintId = complaint.ComplaintId,
            StudentId = complaint.StudentId,
            StudentName = complaint.Student?.FullName ?? string.Empty,
            StudentIndexNumber = complaint.Student?.IndexNumber ?? string.Empty,
            ComplaintCategoryId =
                complaint.ComplaintCategoryId,
            CategoryName =
                complaint.ComplaintCategory?.Name
                ?? string.Empty,
            Description = complaint.Description,
            Status = complaint.Status,
            ResolutionNote =
                complaint.ResolutionNote,
            ResolvedAt = complaint.ResolvedAt,
            StatusChangedByUserId =
                complaint.StatusChangedByUserId
        };

        if (complaint.IsAnonymous)
        {
            response.StudentId = 0;
            response.StudentName = string.Empty;
            response.StudentIndexNumber = string.Empty;
        }

        return response;
    }
}
