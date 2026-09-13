# EF Core migrations

The initial scaffold uses `EnsureCreatedAsync()` only so the project can run before the first migration.

After the database model is approved:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate \
  --project src/CampusServicesPortal.Api \
  --startup-project src/CampusServicesPortal.Api
dotnet ef database update \
  --project src/CampusServicesPortal.Api \
  --startup-project src/CampusServicesPortal.Api
```

Then replace `EnsureCreatedAsync()` in `DatabaseSeedExtensions` with:

```csharp
await dbContext.Database.MigrateAsync();
```

Only the assigned migration owner should create migrations.
