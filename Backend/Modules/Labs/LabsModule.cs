using CampusServicesPortal.Modules.Labs.Interfaces;
using CampusServicesPortal.Modules.Labs.Repositories;
using CampusServicesPortal.Modules.Labs.Services;

namespace CampusServicesPortal.Modules.Labs;

public static class LabsModule
{
    public static IServiceCollection AddLabsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // =====================================================
        // REPOSITORIES
        // =====================================================
        services.AddScoped<
            ILabRepository,
            LabRepository>();

        services.AddScoped<
            ILabTimeSlotRepository,
            LabTimeSlotRepository>();

        services.AddScoped<
            ILabSeatRepository,
            LabSeatRepository>();

        services.AddScoped<
            ILabBookingRepository,
            LabBookingRepository>();


        // =====================================================
        // SERVICES
        // =====================================================
        services.AddScoped<
            ILabService,
            LabService>();

        services.AddScoped<
            ILabTimeSlotService,
            LabTimeSlotService>();

        services.AddScoped<
            ILabSeatService,
            LabSeatService>();

        services.AddScoped<
            ILabBookingService,
            LabBookingService>();


        // =====================================================
        // BACKGROUND HOLD EXPIRY SERVICE
        //
        // Checks every 60 seconds for:
        // Status = Held
        // AND ExpiresAt <= current UTC time
        //
        // Then changes booking status to Expired.
        // =====================================================
        services.AddHostedService<
            LabBookingExpiryBackgroundService>();


        return services;
    }
}