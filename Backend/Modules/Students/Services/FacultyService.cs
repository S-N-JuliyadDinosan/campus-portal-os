using CampusServicesPortal.Common.Exceptions;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Students.DTOs;
using CampusServicesPortal.Modules.Students.Entities;
using CampusServicesPortal.Modules.Students.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Students.Services;

public sealed class FacultyService(ApplicationDbContext dbContext) : IFacultyService
{
    public async Task<IReadOnlyCollection<FacultyResponse>> ListActiveAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Faculties
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Code)
            .Select(x => new FacultyResponse(x.FacultyId, x.Code, x.Name, x.IsActive))
            .ToListAsync(cancellationToken);

    public async Task<FacultyResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var faculty = await dbContext.Faculties.AsNoTracking().SingleOrDefaultAsync(
            x => x.FacultyId == id,
            cancellationToken)
            ?? throw new NotFoundException("Faculty not found.");
        return Map(faculty);
    }

    public async Task<FacultyResponse> CreateAsync(
        CreateFacultyRequest request,
        CancellationToken cancellationToken = default)
    {
        var code = NormalizeCode(request.Code);
        if (await dbContext.Faculties.AnyAsync(x => x.Code == code, cancellationToken))
            throw new BusinessRuleException("Faculty code already exists.");

        var faculty = new Faculty
        {
            Code = code,
            Name = request.Name.Trim(),
            IsActive = true
        };
        dbContext.Faculties.Add(faculty);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(faculty);
    }

    public async Task<FacultyResponse> UpdateAsync(
        int id,
        UpdateFacultyRequest request,
        CancellationToken cancellationToken = default)
    {
        var faculty = await dbContext.Faculties.FindAsync([id], cancellationToken)
            ?? throw new NotFoundException("Faculty not found.");
        var code = NormalizeCode(request.Code);

        if (await dbContext.Faculties.AnyAsync(
            x => x.Code == code && x.FacultyId != id,
            cancellationToken))
        {
            throw new BusinessRuleException("Faculty code already exists.");
        }

        faculty.Code = code;
        faculty.Name = request.Name.Trim();
        faculty.IsActive = request.IsActive;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(faculty);
    }

    public async Task<FacultyDeleteResponse> DeleteOrDeactivateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var faculty = await dbContext.Faculties.FindAsync([id], cancellationToken)
            ?? throw new NotFoundException("Faculty not found.");

        var isReferenced = await dbContext.Students.AnyAsync(x => x.FacultyId == id, cancellationToken) ||
                           await dbContext.StudentMasterList.AnyAsync(x => x.FacultyId == id, cancellationToken);

        if (isReferenced)
        {
            faculty.IsActive = false;
            await dbContext.SaveChangesAsync(cancellationToken);
            return new FacultyDeleteResponse(id, "Deactivated", "Faculty is referenced by student data, so it was safely deactivated instead of deleted.");
        }

        dbContext.Faculties.Remove(faculty);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FacultyDeleteResponse(id, "Deleted", "Unused faculty deleted successfully.");
    }

    private static FacultyResponse Map(Faculty faculty) =>
        new(faculty.FacultyId, faculty.Code, faculty.Name, faculty.IsActive);

    private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();
}
