namespace CampusServicesPortal.Modules.Fees;

public static class FeesModule
{
    public static IServiceCollection AddFeesModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register this module's services and repositories here.
        return services;
    }
}
