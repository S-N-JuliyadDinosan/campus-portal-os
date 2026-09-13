using CampusServicesPortal.Common.Exceptions;
using CampusServicesPortal.Modules.Dashboards.DTOs;
using CampusServicesPortal.Modules.Dashboards.Interfaces;

namespace CampusServicesPortal.Modules.Dashboards.Services;

public sealed class DashboardService(IDashboardRepository repository)
    : IDashboardService
{
    public async Task<StudentDashboardDto> GetStudentDashboardAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        if (!await repository.StudentExistsAsync(studentId, cancellationToken))
            throw new NotFoundException("Student profile not found.");

        return await repository.GetStudentDashboardAsync(
            studentId, cancellationToken);
    }

    public Task<AdminDashboardDto> GetAdminDashboardAsync(
        CancellationToken cancellationToken = default)
    {
        return repository.GetAdminDashboardAsync(cancellationToken);
    }
}
