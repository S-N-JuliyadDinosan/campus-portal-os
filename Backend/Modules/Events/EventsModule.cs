namespace CampusServicesPortal.Modules.Events;

public static class EventsModule
{
    public static IServiceCollection AddEventsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register this module's services and repositories here.
        return services;
    }
}
