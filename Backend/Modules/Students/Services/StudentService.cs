using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Common.Exceptions;
using CampusServicesPortal.Common.Pagination;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Events.Enums;
using CampusServicesPortal.Modules.Identity.Entities;
using CampusServicesPortal.Modules.Identity.Interfaces;
using CampusServicesPortal.Modules.Students.DTOs;
using CampusServicesPortal.Modules.Students.Entities;
using CampusServicesPortal.Modules.Students.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusServicesPortal.Modules.Students.Services;

public sealed class StudentService(
    ApplicationDbContext dbContext,
    IPasswordHasher<User> passwordHasher) : IStudentService
{
    private const int PageSize = 20;

    public async Task<StudentRegistrationResponse> RegisterAsync(
        StudentRegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var indexNumber = request.IndexNumber.Trim().ToUpperInvariant();
        var email = request.Email.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(indexNumber))
            throw new ArgumentException("Index number is required.");

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            if (await dbContext.Users.AnyAsync(
                    x => x.Email == email, cancellationToken))
            {
                throw new BusinessRuleException("Email is already registered.");
            }

            if (await dbContext.Students.AnyAsync(
                    x => x.IndexNumber == indexNumber, cancellationToken))
            {
                throw new BusinessRuleException(
                    "This index number has already been registered.");
            }

            var master = await dbContext.StudentMasterList
                .Include(x => x.Faculty)
                .Include(x => x.Student)
                .SingleOrDefaultAsync(
                    x => x.IndexNumber == indexNumber, cancellationToken);

            if (master is null)
            {
                if (string.IsNullOrWhiteSpace(request.FullName) ||
                    !request.FacultyId.HasValue ||
                    request.FacultyId.Value <= 0)
                {
                    throw new BusinessRuleException(
                        "FullName and FacultyId are required when the index number is not already in the student master list.");
                }

                var faculty = await dbContext.Faculties
                    .SingleOrDefaultAsync(
                        x => x.FacultyId == request.FacultyId.Value && x.IsActive,
                        cancellationToken)
                    ?? throw new BusinessRuleException(
                        "Selected faculty does not exist or is inactive.");

                master = new StudentMaster
                {
                    IndexNumber = indexNumber,
                    FullName = request.FullName.Trim(),
                    OfficialEmail = email,
                    FacultyId = faculty.FacultyId,
                    Faculty = faculty,
                    IntakeYear = DateTime.UtcNow.Year,
                    IsActive = true
                };

                dbContext.StudentMasterList.Add(master);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            else
            {
                if (!master.IsActive)
                    throw new BusinessRuleException(
                        "This student master record is inactive.");

                if (!master.Faculty.IsActive)
                    throw new BusinessRuleException(
                        "The student's faculty is inactive.");

                if (master.Student is not null)
                    throw new BusinessRuleException(
                        "This index number has already been registered.");

                if (!string.IsNullOrWhiteSpace(master.OfficialEmail) &&
                    !string.Equals(
                        master.OfficialEmail.Trim(),
                        email,
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new BusinessRuleException(
                        "Email does not match the official email in the student master list.");
                }
            }

            var user = new User
            {
                Email = email,
                Role = "Student",
                // The BRD does not require an email-verification workflow.
                // Accounts are immediately usable after successful registration.
                EmailVerified = true,
                IsActive = true
            };

            user.PasswordHash = passwordHasher.HashPassword(
                user, request.Password);

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(cancellationToken);

            var student = new Student
            {
                UserId = user.UserId,
                StudentMasterId = master.StudentMasterId,
                FacultyId = master.FacultyId,
                IndexNumber = master.IndexNumber,
                FullName = master.FullName,
                PhoneNumber = Clean(request.PhoneNumber),
                Address = Clean(request.Address)
            };

            dbContext.Students.Add(student);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new StudentRegistrationResponse(
                student.StudentId,
                user.UserId,
                student.IndexNumber,
                student.FullName,
                user.Email,
                false);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    // =========================================================
    // GET MY PROFILE
    // =========================================================
    public async Task<StudentProfileResponse> GetMyProfileAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        var student = await GetStudentQuery()
            .SingleOrDefaultAsync(
                x => x.StudentId == studentId,
                cancellationToken)
            ?? throw new NotFoundException(
                "Student profile not found.");

        return MapProfile(student);
    }


    // =========================================================
    // UPDATE MY PROFILE
    // =========================================================
    public async Task<StudentProfileResponse> UpdateMyProfileAsync(
        int studentId,
        UpdateMyStudentRequest request,
        CancellationToken cancellationToken = default)
    {
        var student = await dbContext.Students
            .Include(x => x.User)
            .Include(x => x.Faculty)
            .SingleOrDefaultAsync(
                x => x.StudentId == studentId,
                cancellationToken)
            ?? throw new NotFoundException(
                "Student profile not found.");

        student.PhoneNumber =
            Clean(request.PhoneNumber);

        student.Address =
            Clean(request.Address);

        await dbContext.SaveChangesAsync(
            cancellationToken);

        return MapProfile(student);
    }


    // =========================================================
    // STUDENT ACTIVITY SUMMARY
    // =========================================================
    public async Task<StudentActivitySummaryResponse> GetActivitySummaryAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Students.AnyAsync(
            x => x.StudentId == studentId,
            cancellationToken))
        {
            throw new NotFoundException(
                "Student profile not found.");
        }

        var hostel =
            await dbContext.HostelApplications.CountAsync(
                x => x.StudentId == studentId,
                cancellationToken);

        var labs =
            await dbContext.LabBookings.CountAsync(
                x => x.StudentId == studentId,
                cancellationToken);

        var events =
            await dbContext.EventRegistrations.CountAsync(
                x => x.StudentId == studentId,
                cancellationToken);

        var complaints =
            await dbContext.Complaints.CountAsync(
                x => x.StudentId == studentId,
                cancellationToken);

        var certificates =
            await dbContext.CertificateRequests.CountAsync(
                x => x.StudentId == studentId,
                cancellationToken);

        var outstanding =
            await dbContext.FeePayments.CountAsync(
                x =>
                    x.StudentId == studentId &&
                    x.Status == FeePaymentStatus.Outstanding,
                cancellationToken);

        return new StudentActivitySummaryResponse(
            hostel,
            labs,
            events,
            complaints,
            certificates,
            outstanding);
    }


    // =========================================================
    // ADMIN - GET STUDENT DETAILS
    // =========================================================
    public async Task<AdminStudentDetailResponse> GetByIdAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        var profile =
            await GetMyProfileAsync(
                studentId,
                cancellationToken);

        var activity =
            await GetActivitySummaryAsync(
                studentId,
                cancellationToken);

        return new AdminStudentDetailResponse(
            profile,
            activity);
    }


    // =========================================================
    // ADMIN - SEARCH STUDENTS
    // =========================================================
    public async Task<PagedResult<StudentListItemResponse>> SearchAsync(
        string? search,
        int? facultyId,
        string? faculty,
        bool? isActive,
        int page,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var query = dbContext.Students
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Faculty)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(
                x =>
                    x.FullName.Contains(term) ||
                    x.IndexNumber.Contains(term) ||
                    x.User.Email.Contains(term));
        }

        if (facultyId.HasValue)
        {
            query = query.Where(
                x => x.FacultyId == facultyId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(faculty))
        {
            var facultyTerm = faculty.Trim();
            query = query.Where(x =>
                x.Faculty.Code.Contains(facultyTerm) ||
                x.Faculty.Name.Contains(facultyTerm));
        }

        if (isActive.HasValue)
        {
            query = query.Where(
                x => x.User.IsActive == isActive.Value);
        }

        var total =
            await query.CountAsync(
                cancellationToken);

        var items =
            await query
                .OrderBy(x => x.IndexNumber)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(
                    x => new StudentListItemResponse(
                        x.StudentId,
                        x.IndexNumber,
                        x.FullName,
                        x.User.Email,
                        x.FacultyId,
                        x.Faculty.Code,
                        x.User.IsActive))
                .ToListAsync(
                    cancellationToken);

        return new PagedResult<StudentListItemResponse>(
            items,
            page,
            PageSize,
            total);
    }


    // =========================================================
    // ADMIN - UPDATE STUDENT
    // =========================================================
    public async Task<StudentProfileResponse> AdminUpdateAsync(
        int studentId,
        AdminUpdateStudentRequest request,
        CancellationToken cancellationToken = default)
    {
        var student = await dbContext.Students
            .Include(x => x.User)
            .Include(x => x.Faculty)
            .SingleOrDefaultAsync(
                x => x.StudentId == studentId,
                cancellationToken)
            ?? throw new NotFoundException(
                "Student not found.");

        if (!string.IsNullOrWhiteSpace(
            request.Email))
        {
            var email =
                request.Email
                    .Trim()
                    .ToLowerInvariant();

            if (await dbContext.Users.AnyAsync(
                x =>
                    x.Email == email &&
                    x.UserId != student.UserId,
                cancellationToken))
            {
                throw new BusinessRuleException(
                    "Email is already in use.");
            }

            if (!string.Equals(
                student.User.Email,
                email,
                StringComparison.OrdinalIgnoreCase))
            {
                student.User.Email = email;

                student.User.EmailVerified =
                    true;

                student.User.SecurityStamp =
                    Guid.NewGuid().ToString("N");
            }
        }

        if (!string.IsNullOrWhiteSpace(
            request.FullName))
        {
            student.FullName =
                request.FullName.Trim();
        }

        if (request.FacultyId.HasValue &&
            request.FacultyId.Value !=
            student.FacultyId)
        {
            var faculty =
                await dbContext.Faculties
                    .SingleOrDefaultAsync(
                        x =>
                            x.FacultyId ==
                            request.FacultyId.Value &&
                            x.IsActive,
                        cancellationToken)
                ?? throw new BusinessRuleException(
                    "Selected faculty does not exist or is inactive.");

            student.FacultyId =
                faculty.FacultyId;

            student.Faculty =
                faculty;
        }

        student.PhoneNumber =
            Clean(request.PhoneNumber);

        student.Address =
            Clean(request.Address);

        await dbContext.SaveChangesAsync(
            cancellationToken);

        return MapProfile(student);
    }


    // =========================================================
    // CHECK DEACTIVATION
    // =========================================================
    public async Task<DeactivationCheckResponse> CheckDeactivationAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Students.AnyAsync(
            x => x.StudentId == studentId,
            cancellationToken))
        {
            throw new NotFoundException(
                "Student not found.");
        }

        var blockers =
            new List<string>();

        var now =
            DateTime.UtcNow;

        var today =
            DateOnly.FromDateTime(now);

        var hostelCount =
            await dbContext.HostelApplications
                .CountAsync(
                    x =>
                        x.StudentId == studentId &&
                        x.Status !=
                        HostelApplicationStatus.Rejected &&
                        x.Status !=
                        HostelApplicationStatus.Cancelled,
                    cancellationToken);

        if (hostelCount > 0)
        {
            blockers.Add(
                $"{hostelCount} active hostel application/allocation(s)");
        }

        var labCount =
            await dbContext.LabBookings
                .CountAsync(
                    x =>
                        x.StudentId == studentId &&
                        x.BookingDate >= today &&
                        (
                            x.Status ==
                            ReservationStatus.Held ||

                            x.Status ==
                            ReservationStatus.Confirmed
                        ),
                    cancellationToken);

        if (labCount > 0)
        {
            blockers.Add(
                $"{labCount} active/future lab booking(s)");
        }

        var eventCount =
            await dbContext.EventRegistrations
                .CountAsync(
                    x =>
                        x.StudentId == studentId &&
                        x.Event.EndAt >= now &&
                        (
                            x.Status ==
                            EventRegistrationStatus.Held ||

                            x.Status ==
                            EventRegistrationStatus.Confirmed
                        ),
                    cancellationToken);

        if (eventCount > 0)
        {
            blockers.Add(
                $"{eventCount} active/future event registration(s)");
        }

        var certificateCount =
            await dbContext.CertificateRequests
                .CountAsync(
                    x =>
                        x.StudentId == studentId &&
                        x.Status !=
                        CertificateRequestStatus.Rejected &&
                        x.Status !=
                        CertificateRequestStatus.Collected,
                    cancellationToken);

        if (certificateCount > 0)
        {
            blockers.Add(
                $"{certificateCount} active certificate request(s)");
        }

        var feeCount =
            await dbContext.FeePayments
                .CountAsync(
                    x =>
                        x.StudentId == studentId &&
                        x.Status ==
                        FeePaymentStatus.Outstanding,
                    cancellationToken);

        if (feeCount > 0)
        {
            blockers.Add(
                $"{feeCount} outstanding fee payment(s)");
        }

        return new DeactivationCheckResponse(
            blockers.Count == 0,
            blockers);
    }


    // =========================================================
    // DEACTIVATE STUDENT
    // =========================================================
    public async Task DeactivateAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        var check =
            await CheckDeactivationAsync(
                studentId,
                cancellationToken);

        if (!check.CanDeactivate)
        {
            throw new BusinessRuleException(
                "Student cannot be deactivated: " +
                string.Join(
                    "; ",
                    check.BlockingCommitments));
        }

        var student =
            await dbContext.Students
                .Include(x => x.User)
                .SingleAsync(
                    x => x.StudentId == studentId,
                    cancellationToken);

        if (!student.User.IsActive)
            return;

        student.User.IsActive =
            false;

        student.User.SecurityStamp =
            Guid.NewGuid().ToString("N");

        student.DeactivatedAt =
            DateTime.UtcNow;

        await RevokeRefreshTokensAsync(
            student.UserId,
            cancellationToken);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }


    // =========================================================
    // REACTIVATE STUDENT
    // =========================================================
    public async Task ReactivateAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        var student =
            await dbContext.Students
                .Include(x => x.User)
                .Include(x => x.StudentMaster)
                .Include(x => x.Faculty)
                .SingleOrDefaultAsync(
                    x => x.StudentId == studentId,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Student not found.");

        if (!student.StudentMaster.IsActive)
        {
            throw new BusinessRuleException(
                "Student master record is inactive. Reactivate it first.");
        }

        if (!student.Faculty.IsActive)
        {
            throw new BusinessRuleException(
                "Student faculty is inactive. Reactivate it first.");
        }

        student.User.IsActive =
            true;

        student.User.SecurityStamp =
            Guid.NewGuid().ToString("N");

        student.DeactivatedAt =
            null;

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }


    // =========================================================
    // STUDENT QUERY
    // =========================================================
    private IQueryable<Student> GetStudentQuery() =>
        dbContext.Students
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Faculty);


    // =========================================================
    // MAP PROFILE
    // =========================================================
    private static StudentProfileResponse MapProfile(
        Student student) =>
        new(
            student.StudentId,
            student.UserId,
            student.IndexNumber,
            student.FullName,
            student.User.Email,
            student.PhoneNumber,
            student.Address,
            student.FacultyId,
            student.Faculty.Code,
            student.Faculty.Name,
            student.User.IsActive,
            student.User.EmailVerified,
            student.DeactivatedAt);


    // =========================================================
    // REVOKE REFRESH TOKENS
    // =========================================================
    private async Task RevokeRefreshTokensAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        var now =
            DateTime.UtcNow;

        var tokens =
            await dbContext.RefreshTokens
                .Where(
                    x =>
                        x.UserId == userId &&
                        x.RevokedAt == null &&
                        x.ExpiresAt > now)
                .ToListAsync(
                    cancellationToken);

        foreach (var token in tokens)
        {
            token.RevokedAt =
                now;
        }
    }


    // =========================================================
    // CLEAN OPTIONAL STRING
    // =========================================================
    private static string? Clean(
        string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}