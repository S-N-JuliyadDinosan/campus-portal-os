using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Dashboards.DTOs;
using CampusServicesPortal.Modules.Dashboards.Interfaces;
using CampusServicesPortal.Modules.Events.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Dashboards.Repositories;

public sealed class DashboardRepository(ApplicationDbContext dbContext)
    : IDashboardRepository
{
    public Task<bool> StudentExistsAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Students.AsNoTracking().AnyAsync(
            x => x.StudentId == studentId,
            cancellationToken);
    }

    public async Task<StudentDashboardDto> GetStudentDashboardAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var now = DateTime.UtcNow;

        var hostelRow = await dbContext.HostelApplications
            .AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new
            {
                x.HostelApplicationId,
                x.Status,
                HostelName = x.PreferredHostel.Name,
                RoomNumber = x.AssignedRoom != null
                    ? x.AssignedRoom.RoomNumber
                    : null
            })
            .FirstOrDefaultAsync(cancellationToken);

        HostelDashboardItem? hostel = hostelRow is null
            ? null
            : new HostelDashboardItem(
                hostelRow.HostelApplicationId,
                hostelRow.Status.ToString(),
                hostelRow.HostelName,
                hostelRow.RoomNumber);

        var labs = await dbContext.LabBookings
            .AsNoTracking()
            .Where(x =>
                x.StudentId == studentId &&
                x.Status == ReservationStatus.Confirmed &&
                x.BookingDate >= today)
            .OrderBy(x => x.BookingDate)
            .ThenBy(x => x.LabTimeSlot.StartTime)
            .Take(10)
            .Select(x => new LabDashboardItem(
                x.LabBookingId,
                x.LabId,
                x.Lab.Name,
                x.BookingDate,
                x.LabTimeSlot.StartTime,
                x.LabTimeSlot.EndTime))
            .ToListAsync(cancellationToken);

        var events = await dbContext.EventRegistrations
            .AsNoTracking()
            .Where(x =>
                x.StudentId == studentId &&
                x.Status == EventRegistrationStatus.Confirmed &&
                x.Event.StartAt >= now)
            .OrderBy(x => x.Event.StartAt)
            .Take(10)
            .Select(x => new EventDashboardItem(
                x.EventRegistrationId,
                x.EventId,
                x.Event.Title,
                x.Event.StartAt,
                x.Event.Venue.Name))
            .ToListAsync(cancellationToken);

        var certificateRows = await dbContext.CertificateRequests
            .AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(10)
            .Select(x => new
            {
                x.CertificateRequestId,
                CertificateType = x.CertificateType.Name,
                x.Status,
                x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var certificates = certificateRows
            .Select(x => new CertificateDashboardItem(
                x.CertificateRequestId,
                x.CertificateType,
                x.Status.ToString(),
                x.CreatedAt))
            .ToList();

        var unread = await dbContext.Notifications
            .AsNoTracking()
            .CountAsync(
                x => x.StudentId == studentId && !x.IsRead,
                cancellationToken);

        return new StudentDashboardDto(
            hostel, labs, events, certificates, unread);
    }

    public async Task<AdminDashboardDto> GetAdminDashboardAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var totalStudents = await dbContext.Students
            .AsNoTracking()
            .CountAsync(x => x.User.IsActive, cancellationToken);

        var pendingHostels = await dbContext.HostelApplications
            .AsNoTracking()
            .CountAsync(
                x => x.Status == HostelApplicationStatus.Pending,
                cancellationToken);

        var complaintRows = await dbContext.Complaints
            .AsNoTracking()
            .Where(x => x.Status != ComplaintStatus.Resolved)
            .GroupBy(x => new { x.ComplaintCategory.Name, x.Status })
            .Select(g => new
            {
                Category = g.Key.Name,
                Status = g.Key.Status,
                Count = g.Count()
            })
            .OrderBy(x => x.Category)
            .ToListAsync(cancellationToken);

        var complaintSummary = complaintRows
            .Select(x => new ComplaintDashboardItem(
                x.Category, x.Status.ToString(), x.Count))
            .ToList();

        var upcomingEvents = await dbContext.Events
            .AsNoTracking()
            .Where(x => x.IsActive && x.IsPublished && x.StartAt >= now)
            .OrderBy(x => x.StartAt)
            .Take(10)
            .Select(x => new AdminEventDashboardItem(
                x.EventId,
                x.Title,
                x.StartAt,
                x.Capacity,
                x.Registrations.Count(r =>
                    r.Status == EventRegistrationStatus.Confirmed)))
            .ToListAsync(cancellationToken);

        var pendingCertificates = await dbContext.CertificateRequests
            .AsNoTracking()
            .CountAsync(
                x => x.Status == CertificateRequestStatus.Pending,
                cancellationToken);

        var paidAmount = await dbContext.FeePayments
            .AsNoTracking()
            .Where(x => x.Status == FeePaymentStatus.Paid)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;

        var outstandingAmount = await dbContext.FeePayments
            .AsNoTracking()
            .Where(x => x.Status == FeePaymentStatus.Outstanding)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;

        var paidCount = await dbContext.FeePayments
            .AsNoTracking()
            .CountAsync(x => x.Status == FeePaymentStatus.Paid, cancellationToken);

        var outstandingCount = await dbContext.FeePayments
            .AsNoTracking()
            .CountAsync(
                x => x.Status == FeePaymentStatus.Outstanding,
                cancellationToken);

        return new AdminDashboardDto(
            totalStudents,
            pendingHostels,
            complaintSummary,
            upcomingEvents,
            pendingCertificates,
            new FeeCollectionSummaryDto(
                paidAmount,
                outstandingAmount,
                paidCount,
                outstandingCount));
    }
}
