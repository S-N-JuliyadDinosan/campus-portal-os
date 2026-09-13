using CampusServicesPortal.Modules.Students.Interfaces;
using CampusServicesPortal.Modules.Students.Services;

namespace CampusServicesPortal.Modules.Students;

public static class StudentsModule
{
    public static IServiceCollection AddStudentsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IAdminAccountService, AdminAccountService>();
        services.AddScoped<IStudentMasterService, StudentMasterService>();
        services.AddScoped<IFacultyService, FacultyService>();
        return services;
    }
}
