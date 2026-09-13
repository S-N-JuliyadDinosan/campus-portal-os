using CampusServicesPortal.Modules.Complaints.Interfaces.Repositories;
using CampusServicesPortal.Modules.Complaints.Interfaces.Services;
using CampusServicesPortal.Modules.Complaints.Repositories;
using CampusServicesPortal.Modules.Complaints.Services;

namespace CampusServicesPortal.Modules.Complaints;

public static class ComplaintsModule
{
    public static IServiceCollection AddComplaintsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<
            IComplaintRepository,
            ComplaintRepository>();

        services.AddScoped<
            IComplaintCategoryRepository,
            ComplaintCategoryRepository>();

        services.AddScoped<
            IComplaintService,
            ComplaintService>();

        services.AddScoped<
            IComplaintCategoryService,
            ComplaintCategoryService>();

        return services;
    }
}