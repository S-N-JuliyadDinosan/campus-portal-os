# Certificate module fixes

- Registered `ICertificateRepository` and `ICertificateService` in `CertificatesModule`.
- Registered `AddCertificatesModule(...)` in `Program.cs`.
- Replaced the singular `api/Certificate/...` controller with REST/BRD routes:
  - `/api/certificate-requests`
  - `/api/certificate-types`
- Added role-based authorization for Student/Admin operations.
- StudentId is now read from JWT claims for new requests; clients cannot impersonate another student.
- Reviewer user id is now read from JWT claims; clients cannot spoof the reviewing admin.
- Added duplicate-pending-request check.
- Added admin filtering by request status and paging.
- Added full Certificate Type CRUD/deactivate flow required by Master Data Management.
- A referenced Certificate Type is deactivated rather than hard deleted.
- Added status-change notification creation in the same explicit DB transaction.
- Replaced `ArgumentException` business-rule errors with project `BusinessRuleException`/`NotFoundException` so middleware returns correct 400/404 responses.
- Added request ownership enforcement for students.
- Replaced inactive FluentValidation-only request validation with MVC DataAnnotations.
