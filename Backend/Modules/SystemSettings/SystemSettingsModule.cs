namespace CampusServicesPortal.Modules.SystemSettings;

public static class SystemSettingsModule
{
    public static IServiceCollection AddSystemSettingsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register this module's services and repositories here.
        return services;
    }
}
