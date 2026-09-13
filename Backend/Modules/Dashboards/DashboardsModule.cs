using CampusServicesPortal.Modules.Dashboards.Interfaces;
using CampusServicesPortal.Modules.Dashboards.Repositories;
using CampusServicesPortal.Modules.Dashboards.Services;

namespace CampusServicesPortal.Modules.Dashboards;

public static class DashboardsModule
{
    public static IServiceCollection AddDashboardsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<IDashboardService, DashboardService>();
        return services;
    }
}
