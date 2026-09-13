namespace CampusServicesPortal.Modules.Dashboards.DTOs;

public sealed record HostelDashboardItem(
    int HostelApplicationId,
    string Status,
    string HostelName,
    string? RoomNumber);

public sealed record LabDashboardItem(
    int LabBookingId,
    int LabId,
    string LabName,
    DateOnly BookingDate,
    TimeOnly StartTime,
    TimeOnly EndTime);

public sealed record EventDashboardItem(
    int EventRegistrationId,
    int EventId,
    string Title,
    DateTime StartAt,
    string Venue);

public sealed record CertificateDashboardItem(
    int CertificateRequestId,
    string CertificateType,
    string Status,
    DateTime RequestedAt);

public sealed record StudentDashboardDto(
    HostelDashboardItem? CurrentHostelApplication,
    IReadOnlyList<LabDashboardItem> UpcomingLabBookings,
    IReadOnlyList<EventDashboardItem> RegisteredEvents,
    IReadOnlyList<CertificateDashboardItem> CertificateRequests,
    int UnreadNotifications);

public sealed record ComplaintDashboardItem(
    string Category,
    string Status,
    int Count);

public sealed record AdminEventDashboardItem(
    int EventId,
    string Title,
    DateTime StartAt,
    int Capacity,
    int RegistrationCount);

public sealed record FeeCollectionSummaryDto(
    decimal PaidAmount,
    decimal OutstandingAmount,
    int PaidCount,
    int OutstandingCount);

public sealed record AdminDashboardDto(
    int TotalRegisteredStudents,
    int PendingHostelApplications,
    IReadOnlyList<ComplaintDashboardItem> OpenComplaints,
    IReadOnlyList<AdminEventDashboardItem> UpcomingEvents,
    int PendingCertificateRequests,
    FeeCollectionSummaryDto FeeCollection);
