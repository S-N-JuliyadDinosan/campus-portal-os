namespace CampusServicesPortal.Modules.Notifications;

public static class NotificationsModule
{
    public static IServiceCollection AddNotificationsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register this module's services and repositories here.
        return services;
    }
}
