# Campus Services Portal — Backend API

ASP.NET Core 8 Web API for the Campus Services Portal BRD (Revision 2.0 backend requirements). The frontend can be Angular 21; the API contract, SQL Server database, JWT authorization and business rules are backend concerns and are independent of the Angular version.

## Technology

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8
- SQL Server
- JWT bearer authentication with Student/Admin roles
- Swagger / OpenAPI
- CORS enabled for `http://localhost:4200`
- Layered modules: Controllers -> Services -> Repositories -> EF Core

## Implemented BRD areas

1. Student Profile
2. Hostel Accommodation
3. Lab Reservation
4. Event Registration
5. Complaint Management
6. Certificate Requests
7. Fee Payment Simulation + receipt data
8. Notifications
9. Student and Administrator dashboards
10. Authentication, authorization, master data and supporting admin endpoints

Core BRD rules are enforced in the API, including unique student email/index number, immutable student index number during profile updates, student ownership checks, administrator-only status changes, hostel approval before room assignment, room-capacity checks, transactional lab double-booking prevention, duplicate/capacity checks for events, duplicate pending certificate prevention, one-time fee payment, and student-only notification access.

## Prerequisites

- .NET 8 SDK
- SQL Server / SQL Server Express / LocalDB
- Visual Studio 2022, Rider, or VS Code (optional)

## 1. Configure the database

Default development connection string in `appsettings.json`:

```text
Server=.\SQLEXPRESS;Database=CampusServicesPortalDb;Trusted_Connection=True;TrustServerCertificate=True
```

Change `ConnectionStrings:DefaultConnection` if your SQL Server instance is different.

The project contains EF Core migrations. In Development, startup calls `Database.MigrateAsync()` and seeds minimal development data automatically.

A standalone setup/seed script is also included at:

```text
Database/CampusServicesPortal.sql
```

## 2. JWT configuration

`appsettings.json` contains a development-only signing key. Replace it for any non-development deployment using configuration, environment variables, or user secrets.

Relevant settings:

```json
"Jwt": {
  "Issuer": "CampusServicesPortal",
  "Audience": "CampusServicesPortal.Client",
  "Key": "DEVELOPMENT-ONLY-CHANGE-USING-USER-SECRETS-1234567890",
  "AccessTokenMinutes": 480,
  "RefreshTokenDays": 7
}
```

## 3. Run

From the project folder:

```bash
dotnet restore
dotnet build
dotnet run
```

Development URLs from `Properties/launchSettings.json`:

```text
http://localhost:5280
https://localhost:7281
```

Swagger:

```text
http://localhost:5280/swagger
```

## 4. Seeded development admin

The Development seeder creates an administrator only when no Admin user exists:

```text
Email:    admin@campus.local
Password: Admin@123
```

Change this password before using the project outside local development.

## 5. Student registration

BRD endpoint:

```text
POST /api/students/register
```

The API supports both scenarios:

- If the index number already exists in the optional Student Master List, its faculty/name data is used and checked.
- If it does not exist in the master list, the request must provide `FullName` and `FacultyId`, allowing normal BRD registration without requiring pre-imported master data.

Email and index number uniqueness are enforced server-side. Students cannot update their own index number after registration.

Email verification is not required by the BRD and is disabled by default with:

```json
"Identity": {
  "RequireEmailVerification": false
}
```

The existing verification/reset endpoints are retained as optional supporting functionality.

## 6. Main BRD routes

### Students

```text
POST   /api/students/register
GET    /api/students/{id}
PUT    /api/students/{id}
GET    /api/students?search=&faculty=
DELETE /api/students/{id}
```

### Hostel accommodation

```text
POST /api/hostel-applications
GET  /api/hostel-applications/student/{studentId}
GET  /api/hostel-applications?status=
PUT  /api/hostel-applications/{id}/status
PUT  /api/hostel-applications/{id}/assign-room
```

### Labs

```text
GET    /api/labs
GET    /api/labs/{id}/slots?date=YYYY-MM-DD
POST   /api/lab-bookings
DELETE /api/lab-bookings/{id}
GET    /api/lab-bookings/student/{studentId}
```

### Events

```text
GET    /api/events
POST   /api/event-registrations
DELETE /api/event-registrations/{id}
GET    /api/event-registrations/student/{studentId}
```

### Complaints

```text
GET  /api/complaint-categories
POST /api/complaints
GET  /api/complaints/student/{studentId}
GET  /api/complaints?status=
PUT  /api/complaints/{id}/status
```

### Certificates

```text
POST /api/certificate-requests
GET  /api/certificate-requests/student/{studentId}
GET  /api/certificate-requests?status=
PUT  /api/certificate-requests/{id}/status
```

### Fees

```text
GET  /api/fee-payments/student/{studentId}
POST /api/fee-payments/{id}/pay
GET  /api/fee-payments/{id}/receipt
```

### Notifications

```text
GET /api/notifications/student/{studentId}
PUT /api/notifications/{id}/read
```

### Dashboards

```text
GET /api/dashboard/student
GET /api/dashboard/admin
GET /api/dashboard
```

Additional CRUD/master-data endpoints are available in Swagger for hostels, rooms, labs, lab seats/time slots, events, venues/seats, complaint categories, certificate types, fee types, faculties, student master data, administrator accounts and system settings.

## 7. CORS for Angular 21

Default allowed origin:

```text
http://localhost:4200
```

Override with `Cors:AllowedOrigins` when the frontend is hosted elsewhere.

## 8. Database setup notes

- EF migrations are in `Data/Migrations`.
- Development startup applies pending migrations automatically.
- Minimal seed data includes faculties, hostel/rooms, labs/time slots/seats, complaint categories, certificate types, fee types, system settings and one Admin account.
- `Database/CampusServicesPortal.sql` can be used when a SQL script is preferred.

## 9. Production checklist

Before deployment:

- Replace the development JWT signing key.
- Replace the seeded Admin password or remove the development bootstrap account.
- Configure a production SQL Server connection string.
- Configure the production CORS origin.
- Enable HTTPS and production-safe logging.
- Configure SMTP only if the optional email features are used.
- Apply migrations explicitly as part of deployment.

