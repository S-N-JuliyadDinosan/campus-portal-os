using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Hostels.Entities;
using CampusServicesPortal.Modules.Hostels.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Hostels.Repositories;

public sealed class HostelApplicationRepository
    : IHostelApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public HostelApplicationRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // GET BY ID
    // =========================================================
    public async Task<HostelApplication?> GetByIdAsync(
        int hostelApplicationId)
    {
        return await _context.HostelApplications
            .Include(x => x.PreferredHostel)
            .Include(x => x.AssignedRoom)
            .Include(x => x.Student)
            .FirstOrDefaultAsync(
                x =>
                    x.HostelApplicationId ==
                    hostelApplicationId);
    }


    // =========================================================
    // GET APPLICATIONS BY STUDENT
    // =========================================================
    public async Task<List<HostelApplication>>
        GetByStudentIdAsync(
            int studentId)
    {
        return await _context.HostelApplications
            .AsNoTracking()
            .Include(x => x.PreferredHostel)
            .Include(x => x.AssignedRoom)
            .Where(x =>
                x.StudentId == studentId)
            .OrderByDescending(x =>
                x.CreatedAt)
            .ToListAsync();
    }


    // =========================================================
    // CHECK ACTIVE APPLICATION
    //
    // Pending
    // Approved
    // RoomAssigned
    //
    // are considered active.
    // =========================================================
    public async Task<bool> HasActiveApplicationAsync(
        int studentId,
        string academicYear,
        string semester)
    {
        return await _context.HostelApplications
            .AnyAsync(x =>
                x.StudentId == studentId &&
                x.AcademicYear == academicYear &&
                x.Semester == semester &&
                (
                    x.Status ==
                        HostelApplicationStatus.Pending
                    ||
                    x.Status ==
                        HostelApplicationStatus.Approved
                    ||
                    x.Status ==
                        HostelApplicationStatus.RoomAssigned
                ));
    }


    // =========================================================
    // ADD APPLICATION
    // =========================================================
    public async Task<HostelApplication> AddAsync(
        HostelApplication application)
    {
        _context.HostelApplications.Add(
            application);

        await _context.SaveChangesAsync();

        return application;
    }


    // =========================================================
    // UPDATE APPLICATION
    // =========================================================
    public async Task UpdateAsync(
        HostelApplication application)
    {
        _context.HostelApplications.Update(
            application);

        await _context.SaveChangesAsync();
    }


    // =========================================================
    // GET ALL APPLICATIONS
    // =========================================================
    public async Task<List<HostelApplication>> GetAllAsync(
        int? hostelId = null,
        string? status = null,
        string? academicYear = null,
        string? semester = null,
        int page = 1)
    {
        const int pageSize = 20;

        if (page < 1)
        {
            page = 1;
        }

        var query =
            _context.HostelApplications
                .AsNoTracking()
                .Include(x => x.PreferredHostel)
                .Include(x => x.AssignedRoom)
                .Include(x => x.Student)
                .AsQueryable();


        // -----------------------------------------------------
        // FILTER BY HOSTEL
        // -----------------------------------------------------
        if (hostelId.HasValue)
        {
            query = query.Where(
                x =>
                    x.PreferredHostelId ==
                    hostelId.Value);
        }


        // -----------------------------------------------------
        // FILTER BY STATUS
        // -----------------------------------------------------
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<HostelApplicationStatus>(
                    status.Trim(),
                    true,
                    out var parsedStatus))
            {
                query = query.Where(
                    x =>
                        x.Status ==
                        parsedStatus);
            }
        }


        // -----------------------------------------------------
        // FILTER BY ACADEMIC YEAR
        // -----------------------------------------------------
        if (!string.IsNullOrWhiteSpace(
                academicYear))
        {
            var normalizedAcademicYear =
                academicYear.Trim();

            query = query.Where(
                x =>
                    x.AcademicYear ==
                    normalizedAcademicYear);
        }


        // -----------------------------------------------------
        // FILTER BY SEMESTER
        // -----------------------------------------------------
        if (!string.IsNullOrWhiteSpace(
                semester))
        {
            var normalizedSemester =
                semester.Trim();

            query = query.Where(
                x =>
                    x.Semester ==
                    normalizedSemester);
        }


        return await query
            .OrderByDescending(
                x => x.CreatedAt)
            .Skip(
                (page - 1) *
                pageSize)
            .Take(
                pageSize)
            .ToListAsync();
    }


    // =========================================================
    // GET ASSIGNED STUDENT COUNT BY HOSTEL
    // =========================================================
    public async Task<int> GetAssignedCountByHostelAsync(
        int hostelId,
        string academicYear,
        string semester)
    {
        return await _context.HostelApplications
            .CountAsync(x =>
                x.PreferredHostelId ==
                    hostelId
                &&
                x.AcademicYear ==
                    academicYear
                &&
                x.Semester ==
                    semester
                &&
                x.AssignedRoomId != null
                &&
                (
                    x.Status ==
                        HostelApplicationStatus.RoomAssigned
                    ||
                    x.Status ==
                        HostelApplicationStatus.Approved
                ));
    }


    // =========================================================
    // GET OCCUPIED COUNT FOR ONE ROOM
    // =========================================================
    public async Task<int> GetAssignedCountAsync(
        int roomId)
    {
        return await _context.HostelApplications
            .CountAsync(x =>
                x.AssignedRoomId ==
                    roomId
                &&
                (
                    x.Status ==
                        HostelApplicationStatus.RoomAssigned
                    ||
                    x.Status ==
                        HostelApplicationStatus.Approved
                ));
    }
}