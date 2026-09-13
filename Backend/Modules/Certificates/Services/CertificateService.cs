using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Common.Exceptions;
using CampusServicesPortal.Common.Security;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Certificates.DTOs;
using CampusServicesPortal.Modules.Certificates.Entities;
using CampusServicesPortal.Modules.Certificates.Interfaces;
using CampusServicesPortal.Modules.Notifications.DTOs;
using CampusServicesPortal.Modules.Notifications.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Certificates.Services;

public class CertificateService : ICertificateService
{
    private readonly ICertificateRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notificationService;
    private readonly ApplicationDbContext _dbContext;

    public CertificateService(
        ICertificateRepository repository,
        ICurrentUserService currentUser,
        INotificationService notificationService,
        ApplicationDbContext dbContext)
    {
        _repository = repository;
        _currentUser = currentUser;
        _notificationService = notificationService;
        _dbContext = dbContext;
    }

    public async Task<List<CertificateTypeDto>> GetCertificateTypesAsync()
    {
        var activeOnly = !IsAdmin();
        var types = await _repository.GetCertificateTypesAsync(activeOnly);
        return types.Select(MapTypeToDto).ToList();
    }

    public async Task<CertificateTypeDto?> GetCertificateTypeByIdAsync(int id)
    {
        var type = await _repository.GetCertificateTypeByIdAsync(id);

        if (type is null)
        {
            return null;
        }

        if (!IsAdmin() && !type.IsActive)
        {
            return null;
        }

        return MapTypeToDto(type);
    }

    public async Task<CertificateTypeDto> CreateCertificateTypeAsync(
        CreateCertificateTypeDto dto)
    {
        var name = dto.Name.Trim();

        if (await _repository.CertificateTypeNameExistsAsync(name))
        {
            throw new BusinessRuleException(
                "A certificate type with this name already exists.");
        }

        var type = new CertificateType
        {
            Name = name,
            Description = dto.Description?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateCertificateTypeAsync(type);
        return MapTypeToDto(created);
    }

    public async Task<CertificateTypeDto> UpdateCertificateTypeAsync(
        int id,
        UpdateCertificateTypeDto dto)
    {
        var type = await _repository.GetCertificateTypeByIdAsync(id, tracking: true)
            ?? throw new NotFoundException("Certificate type was not found.");

        var name = dto.Name.Trim();

        if (await _repository.CertificateTypeNameExistsAsync(name, id))
        {
            throw new BusinessRuleException(
                "A certificate type with this name already exists.");
        }

        type.Name = name;
        type.Description = dto.Description?.Trim();
        type.IsActive = dto.IsActive;

        var updated = await _repository.UpdateCertificateTypeAsync(type);
        return MapTypeToDto(updated);
    }

    public async Task DeleteOrDeactivateCertificateTypeAsync(int id)
    {
        var type = await _repository.GetCertificateTypeByIdAsync(id, tracking: true)
            ?? throw new NotFoundException("Certificate type was not found.");

        var hasRequests = await _repository.CertificateTypeHasRequestsAsync(id);

        if (hasRequests)
        {
            type.IsActive = false;
            await _repository.UpdateCertificateTypeAsync(type);
            return;
        }

        await _repository.DeleteCertificateTypeAsync(type);
    }

    public async Task<List<CertificateRequestDto>> GetAllRequestsAsync(
        string? status,
        int page = 1)
    {
        CertificateRequestStatus? parsedStatus = null;

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<CertificateRequestStatus>(
                    status,
                    ignoreCase: true,
                    out var value))
            {
                throw new BusinessRuleException(
                    "Invalid certificate request status. " +
                    "Use Pending, Approved, Rejected, ReadyForCollection or Collected.");
            }

            parsedStatus = value;
        }

        var requests = await _repository.GetAllRequestsAsync(parsedStatus, page);
        return requests.Select(MapRequestToDto).ToList();
    }

    public async Task<CertificateRequestDto?> GetRequestByIdAsync(int id)
    {
        var request = await _repository.GetRequestByIdAsync(id);

        if (request is null)
        {
            return null;
        }

        EnsureCanViewStudentRequest(request.StudentId);
        return MapRequestToDto(request);
    }

    public async Task<List<CertificateRequestDto>> GetMyRequestsAsync()
    {
        var studentId = RequireStudentId();
        var requests = await _repository.GetRequestsByStudentIdAsync(studentId);
        return requests.Select(MapRequestToDto).ToList();
    }

    public async Task<List<CertificateRequestDto>> GetRequestsByStudentIdAsync(int studentId)
    {
        EnsureCanViewStudentRequest(studentId);

        var requests = await _repository.GetRequestsByStudentIdAsync(studentId);
        return requests.Select(MapRequestToDto).ToList();
    }

    public async Task<CertificateRequestDto> CreateRequestAsync(
        CreateCertificateRequestDto dto)
    {
        var studentId = RequireStudentId();

        var certificateType = await _repository.GetCertificateTypeByIdAsync(
            dto.CertificateTypeId);

        if (certificateType is null)
        {
            throw new NotFoundException("Certificate type was not found.");
        }

        if (!certificateType.IsActive)
        {
            throw new BusinessRuleException(
                "This certificate type is currently inactive.");
        }

        if (await _repository.HasPendingRequestAsync(studentId, dto.CertificateTypeId))
        {
            throw new BusinessRuleException(
                "You already have a pending request for this certificate type.");
        }

        var request = new CertificateRequest
        {
            CertificateTypeId = dto.CertificateTypeId,
            StudentId = studentId,
            Reason = dto.Reason?.Trim(),
            Status = CertificateRequestStatus.Pending
        };

        await _repository.CreateRequestAsync(request);

        var created = await _repository.GetRequestByIdAsync(request.CertificateRequestId)
            ?? throw new InvalidOperationException(
                "Certificate request was created but could not be reloaded.");

        return MapRequestToDto(created);
    }

    public async Task<CertificateRequestDto> UpdateRequestStatusAsync(
        int id,
        UpdateCertificateRequestDto dto)
    {
        if (!Enum.TryParse<CertificateRequestStatus>(
                dto.Status,
                ignoreCase: true,
                out var status))
        {
            throw new BusinessRuleException(
                "Invalid certificate request status. " +
                "Use Pending, Approved, Rejected, ReadyForCollection or Collected.");
        }

        var adminUserId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException(
                "Authenticated administrator identity was not found.");

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var request = await _repository.GetRequestByIdAsync(id, tracking: true)
                ?? throw new NotFoundException("Certificate request was not found.");

            request.Status = status;
            request.ReviewNote = string.IsNullOrWhiteSpace(dto.ReviewNote)
                ? null
                : dto.ReviewNote.Trim();
            request.ReviewedByUserId = adminUserId;
            request.ReviewedAt = DateTime.UtcNow;

            await _repository.UpdateRequestAsync(request);

            await _notificationService.CreateAsync(
                new CreateNotificationDto
                {
                    StudentId = request.StudentId,
                    Type = "CertificateUpdated",
                    Title = "Certificate request updated",
                    Message = BuildStatusMessage(
                        request.CertificateType?.Name ?? "certificate",
                        status)
                });

            await transaction.CommitAsync();

            var updated = await _repository.GetRequestByIdAsync(id)
                ?? throw new InvalidOperationException(
                    "Certificate request was updated but could not be reloaded.");

            return MapRequestToDto(updated);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private int RequireStudentId()
    {
        return _currentUser.StudentId
            ?? throw new UnauthorizedAccessException(
                "Authenticated student identity was not found.");
    }

    private void EnsureCanViewStudentRequest(int studentId)
    {
        if (IsAdmin())
        {
            return;
        }

        var currentStudentId = RequireStudentId();

        if (currentStudentId != studentId)
        {
            throw new UnauthorizedAccessException(
                "You can only view your own certificate requests.");
        }
    }

    private bool IsAdmin()
    {
        return string.Equals(
            _currentUser.Role,
            "Admin",
            StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildStatusMessage(
        string certificateTypeName,
        CertificateRequestStatus status)
    {
        return status switch
        {
            CertificateRequestStatus.Approved =>
                $"Your {certificateTypeName} request has been approved.",
            CertificateRequestStatus.Rejected =>
                $"Your {certificateTypeName} request has been rejected.",
            CertificateRequestStatus.ReadyForCollection =>
                $"Your {certificateTypeName} is ready for collection.",
            CertificateRequestStatus.Collected =>
                $"Your {certificateTypeName} request has been marked as collected.",
            _ =>
                $"Your {certificateTypeName} request status is now {status}."
        };
    }

    private static CertificateTypeDto MapTypeToDto(CertificateType type)
    {
        return new CertificateTypeDto
        {
            CertificateTypeId = type.CertificateTypeId,
            Name = type.Name,
            Description = type.Description,
            IsActive = type.IsActive,
            CreatedAt = type.CreatedAt
        };
    }

    private static CertificateRequestDto MapRequestToDto(
    CertificateRequest request)
    {
        return new CertificateRequestDto
        {
            CertificateRequestId =
                request.CertificateRequestId,

            CertificateTypeId =
                request.CertificateTypeId,

            CertificateTypeName =
                request.CertificateType?.Name ?? string.Empty,

            StudentId =
                request.StudentId,

            StudentName =
                request.Student?.FullName ?? string.Empty,

            ReviewedByUserId =
                request.ReviewedByUserId,

            Reason =
                request.Reason,

            Status =
                request.Status.ToString(),

            ReviewNote =
                request.ReviewNote,

            RequestedAt =
                request.CreatedAt,

            ReviewedAt =
                request.ReviewedAt
        };
    }
}
