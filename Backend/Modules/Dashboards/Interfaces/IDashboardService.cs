using CampusServicesPortal.Modules.Dashboards.DTOs;

namespace CampusServicesPortal.Modules.Dashboards.Interfaces;

public interface IDashboardService
{
    Task<StudentDashboardDto> GetStudentDashboardAsync(
        int studentId,
        CancellationToken cancellationToken = default);

    Task<AdminDashboardDto> GetAdminDashboardAsync(
        CancellationToken cancellationToken = default);
}
