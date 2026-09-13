using CampusServicesPortal.Modules.Hostels.Interfaces;
using CampusServicesPortal.Modules.Hostels.Repositories;
using CampusServicesPortal.Modules.Hostels.Services;

namespace CampusServicesPortal.Modules.Hostels;

public static class HostelsModule
{
    public static IServiceCollection AddHostelsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Repositories
        services.AddScoped<IHostelRepository, HostelRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<
            IHostelApplicationRepository,
            HostelApplicationRepository>();

        // Services
        services.AddScoped<IHostelService, HostelService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<
            IHostelApplicationService,
            HostelApplicationService>();

        return services;
    }
}