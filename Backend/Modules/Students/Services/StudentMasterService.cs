using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using CampusServicesPortal.Common.Exceptions;
using CampusServicesPortal.Common.Pagination;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Students.DTOs;
using CampusServicesPortal.Modules.Students.Entities;
using CampusServicesPortal.Modules.Students.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Students.Services;

public sealed class StudentMasterService(ApplicationDbContext dbContext) : IStudentMasterService
{
    private const int PageSize = 20;

    public async Task<StudentMasterCheckResponse> CheckAsync(
        string indexNumber,
        CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeIndex(indexNumber);
        var record = await dbContext.StudentMasterList
            .AsNoTracking()
            .Include(x => x.Student)
            .SingleOrDefaultAsync(x => x.IndexNumber == normalized, cancellationToken);

        return record is null
            ? new StudentMasterCheckResponse(normalized, false, false, false)
            : new StudentMasterCheckResponse(normalized, true, record.IsActive, record.Student is not null);
    }

    public async Task<PagedResult<StudentMasterResponse>> SearchAsync(
        string? search,
        int? facultyId,
        int? intakeYear,
        bool? isActive,
        int page,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        var query = dbContext.StudentMasterList
            .AsNoTracking()
            .Include(x => x.Faculty)
            .Include(x => x.Student)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x =>
                x.IndexNumber.Contains(term) ||
                x.FullName.Contains(term) ||
                (x.OfficialEmail != null && x.OfficialEmail.Contains(term)));
        }

        if (facultyId.HasValue)
            query = query.Where(x => x.FacultyId == facultyId.Value);
        if (intakeYear.HasValue)
            query = query.Where(x => x.IntakeYear == intakeYear.Value);
        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.IndexNumber)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(x => new StudentMasterResponse(
                x.StudentMasterId,
                x.IndexNumber,
                x.FullName,
                x.OfficialEmail,
                x.FacultyId,
                x.Faculty.Code,
                x.Faculty.Name,
                x.IntakeYear,
                x.IsActive,
                x.Student != null))
            .ToListAsync(cancellationToken);

        return new PagedResult<StudentMasterResponse>(items, page, PageSize, total);
    }

    public async Task<StudentMasterResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var record = await QueryWithDetails()
            .SingleOrDefaultAsync(x => x.StudentMasterId == id, cancellationToken)
            ?? throw new NotFoundException("Student master record not found.");
        return Map(record);
    }

    public async Task<StudentMasterResponse> CreateAsync(
        CreateStudentMasterRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateIntakeYear(request.IntakeYear);
        var index = NormalizeIndex(request.IndexNumber);
        var email = NormalizeOptionalEmail(request.OfficialEmail);

        if (await dbContext.StudentMasterList.AnyAsync(x => x.IndexNumber == index, cancellationToken))
            throw new BusinessRuleException("Index number already exists in the student master list.");

        var faculty = await RequireFacultyAsync(request.FacultyId, cancellationToken);
        var record = new StudentMaster
        {
            IndexNumber = index,
            FullName = request.FullName.Trim(),
            OfficialEmail = email,
            FacultyId = faculty.FacultyId,
            IntakeYear = request.IntakeYear,
            IsActive = request.IsActive,
            Faculty = faculty
        };

        dbContext.StudentMasterList.Add(record);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(record);
    }

    public async Task<StudentMasterResponse> UpdateAsync(
        int id,
        UpdateStudentMasterRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateIntakeYear(request.IntakeYear);
        var record = await dbContext.StudentMasterList
            .Include(x => x.Faculty)
            .Include(x => x.Student)
            .SingleOrDefaultAsync(x => x.StudentMasterId == id, cancellationToken)
            ?? throw new NotFoundException("Student master record not found.");

        var index = NormalizeIndex(request.IndexNumber);
        if (await dbContext.StudentMasterList.AnyAsync(
            x => x.IndexNumber == index && x.StudentMasterId != id,
            cancellationToken))
        {
            throw new BusinessRuleException("Index number already exists in the student master list.");
        }

        var faculty = await RequireFacultyAsync(request.FacultyId, cancellationToken);
        record.IndexNumber = index;
        record.FullName = request.FullName.Trim();
        record.OfficialEmail = NormalizeOptionalEmail(request.OfficialEmail);
        record.FacultyId = faculty.FacultyId;
        record.Faculty = faculty;
        record.IntakeYear = request.IntakeYear;
        record.IsActive = request.IsActive;

        // Keep the already-registered Student row consistent with corrected official master data.
        if (record.Student is not null)
        {
            record.Student.IndexNumber = index;
            record.Student.FullName = record.FullName;
            record.Student.FacultyId = faculty.FacultyId;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(record);
    }

    public async Task DeactivateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var record = await dbContext.StudentMasterList.FindAsync([id], cancellationToken)
            ?? throw new NotFoundException("Student master record not found.");
        record.IsActive = false;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<StudentMasterImportResponse> ImportCsvAsync(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
            throw new BusinessRuleException("Please upload a non-empty CSV file.");

        if (!string.Equals(Path.GetExtension(file.FileName), ".csv", StringComparison.OrdinalIgnoreCase))
            throw new BusinessRuleException("Only CSV files are supported.");

        List<string[]> rows;
        await using (var stream = file.OpenReadStream())
        using (var reader = new StreamReader(stream, Encoding.UTF8, true))
        {
            rows = ParseCsv(await reader.ReadToEndAsync());
        }

        if (rows.Count < 2)
            throw new BusinessRuleException("CSV must contain a header row and at least one data row.");

        var headers = rows[0]
            .Select((value, index) => new { Key = NormalizeHeader(value), Index = index })
            .GroupBy(x => x.Key)
            .ToDictionary(g => g.Key, g => g.First().Index, StringComparer.OrdinalIgnoreCase);

        var required = new[] { "indexnumber", "fullname", "facultycode", "intakeyear" };
        var missing = required.Where(x => !headers.ContainsKey(x)).ToList();
        if (missing.Count > 0)
            throw new BusinessRuleException("Missing CSV column(s): " + string.Join(", ", missing));

        var facultyList = await dbContext.Faculties.AsNoTracking().ToListAsync(cancellationToken);
        var faculties = facultyList.ToDictionary(x => x.Code.ToUpperInvariant(), x => x, StringComparer.OrdinalIgnoreCase);

        var parsed = new List<ImportRow>();
        var errors = new List<string>();
        var seenIndexes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var emailValidator = new EmailAddressAttribute();

        for (var i = 1; i < rows.Count; i++)
        {
            var rowNumber = i + 1;
            var row = rows[i];
            if (row.All(string.IsNullOrWhiteSpace)) continue;

            string Get(string key) => headers.TryGetValue(key, out var col) && col < row.Length ? row[col].Trim() : string.Empty;

            var index = NormalizeIndex(Get("indexnumber"));
            var fullName = Get("fullname");
            var facultyCode = Get("facultycode").ToUpperInvariant();
            var intakeText = Get("intakeyear");
            var officialEmail = NormalizeOptionalEmail(Get("officialemail"));
            var isActiveText = Get("isactive");

            if (string.IsNullOrWhiteSpace(index)) errors.Add($"Row {rowNumber}: IndexNumber is required.");
            if (string.IsNullOrWhiteSpace(fullName)) errors.Add($"Row {rowNumber}: FullName is required.");
            if (!seenIndexes.Add(index)) errors.Add($"Row {rowNumber}: duplicate IndexNumber '{index}' in the CSV.");
            if (!faculties.TryGetValue(facultyCode, out var faculty))
                errors.Add($"Row {rowNumber}: FacultyCode '{facultyCode}' was not found.");
            else if (!faculty.IsActive)
                errors.Add($"Row {rowNumber}: FacultyCode '{facultyCode}' is inactive.");
            if (!int.TryParse(intakeText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intakeYear) || intakeYear < 2000 || intakeYear > 2100)
                errors.Add($"Row {rowNumber}: IntakeYear must be between 2000 and 2100.");
            if (officialEmail is not null && !emailValidator.IsValid(officialEmail))
                errors.Add($"Row {rowNumber}: OfficialEmail is invalid.");

            var isActive = true;
            if (!string.IsNullOrWhiteSpace(isActiveText) && !bool.TryParse(isActiveText, out isActive))
                errors.Add($"Row {rowNumber}: IsActive must be true or false.");

            if (faculty is not null && intakeYear is >= 2000 and <= 2100 && !string.IsNullOrWhiteSpace(index) && !string.IsNullOrWhiteSpace(fullName))
            {
                parsed.Add(new ImportRow(index, fullName, officialEmail, faculty.FacultyId, intakeYear, isActive));
            }
        }

        if (errors.Count > 0)
            return new StudentMasterImportResponse(false, rows.Count - 1, 0, 0, errors);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var indexes = parsed.Select(x => x.IndexNumber).ToList();
        var existingList = await dbContext.StudentMasterList
            .Include(x => x.Student)
            .Where(x => indexes.Contains(x.IndexNumber))
            .ToListAsync(cancellationToken);
        var existing = existingList.ToDictionary(x => x.IndexNumber, StringComparer.OrdinalIgnoreCase);

        var inserted = 0;
        var updated = 0;

        foreach (var item in parsed)
        {
            if (existing.TryGetValue(item.IndexNumber, out var record))
            {
                record.FullName = item.FullName;
                record.OfficialEmail = item.OfficialEmail;
                record.FacultyId = item.FacultyId;
                record.IntakeYear = item.IntakeYear;
                record.IsActive = item.IsActive;
                if (record.Student is not null)
                {
                    record.Student.FullName = item.FullName;
                    record.Student.FacultyId = item.FacultyId;
                }
                updated++;
            }
            else
            {
                dbContext.StudentMasterList.Add(new StudentMaster
                {
                    IndexNumber = item.IndexNumber,
                    FullName = item.FullName,
                    OfficialEmail = item.OfficialEmail,
                    FacultyId = item.FacultyId,
                    IntakeYear = item.IntakeYear,
                    IsActive = item.IsActive
                });
                inserted++;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new StudentMasterImportResponse(true, parsed.Count, inserted, updated, Array.Empty<string>());
    }

    private IQueryable<StudentMaster> QueryWithDetails() =>
        dbContext.StudentMasterList
            .AsNoTracking()
            .Include(x => x.Faculty)
            .Include(x => x.Student);

    private static StudentMasterResponse Map(StudentMaster record) =>
        new(
            record.StudentMasterId,
            record.IndexNumber,
            record.FullName,
            record.OfficialEmail,
            record.FacultyId,
            record.Faculty.Code,
            record.Faculty.Name,
            record.IntakeYear,
            record.IsActive,
            record.Student is not null);

    private async Task<Faculty> RequireFacultyAsync(int facultyId, CancellationToken cancellationToken) =>
        await dbContext.Faculties.SingleOrDefaultAsync(x => x.FacultyId == facultyId && x.IsActive, cancellationToken)
        ?? throw new BusinessRuleException("Faculty not found or inactive.");

    private static void ValidateIntakeYear(int year)
    {
        if (year < 2000 || year > 2100)
            throw new BusinessRuleException("Intake year must be between 2000 and 2100.");
    }

    private static string NormalizeIndex(string value) =>
        (value ?? string.Empty).Trim().ToUpperInvariant();

    private static string? NormalizeOptionalEmail(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();

    private static string NormalizeHeader(string value) =>
        new string((value ?? string.Empty).Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();

    private static List<string[]> ParseCsv(string text)
    {
        var rows = new List<string[]>();
        var row = new List<string>();
        var field = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < text.Length && text[i + 1] == '"')
                {
                    field.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                row.Add(field.ToString());
                field.Clear();
            }
            else if ((c == '\r' || c == '\n') && !inQuotes)
            {
                if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                row.Add(field.ToString());
                field.Clear();
                rows.Add(row.ToArray());
                row.Clear();
            }
            else
            {
                field.Append(c);
            }
        }

        if (field.Length > 0 || row.Count > 0)
        {
            row.Add(field.ToString());
            rows.Add(row.ToArray());
        }

        return rows;
    }

    private sealed record ImportRow(
        string IndexNumber,
        string FullName,
        string? OfficialEmail,
        int FacultyId,
        int IntakeYear,
        bool IsActive);
}
