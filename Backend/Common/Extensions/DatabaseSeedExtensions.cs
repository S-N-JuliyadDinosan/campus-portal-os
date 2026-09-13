using CampusServicesPortal.Data;
using CampusServicesPortal.Data.SeedData;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Common.Extensions;

public static class DatabaseSeedExtensions
{
    public static async Task SeedDatabaseAsync(
        this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var environment = scope.ServiceProvider
            .GetRequiredService<IHostEnvironment>();

        if (!environment.IsDevelopment())
        {
            return;
        }

        var dbContext = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        // Use EF Core migrations instead of EnsureCreated.
        // This makes database changes portable to other PCs/servers.
        await dbContext.Database.MigrateAsync();

        // Add/update default seed data after migrations are applied.
        await DatabaseSeeder.SeedAsync(dbContext);
    }
}