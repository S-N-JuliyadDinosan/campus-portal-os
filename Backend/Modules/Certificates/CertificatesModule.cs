using CampusServicesPortal.Modules.Certificates.Interfaces;
using CampusServicesPortal.Modules.Certificates.Repositories;
using CampusServicesPortal.Modules.Certificates.Services;

namespace CampusServicesPortal.Modules.Certificates;

public static class CertificatesModule
{
    public static IServiceCollection AddCertificatesModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ICertificateRepository, CertificateRepository>();
        services.AddScoped<ICertificateService, CertificateService>();

        return services;
    }
}
