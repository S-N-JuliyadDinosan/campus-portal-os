using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Certificates.Entities;
using CampusServicesPortal.Modules.Certificates.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Certificates.Repositories;

public class CertificateRepository : ICertificateRepository
{
    private const int PageSize = 20;

    private readonly ApplicationDbContext _context;

    public CertificateRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // CERTIFICATE TYPES
    // =========================================================

    public async Task<List<CertificateType>> GetCertificateTypesAsync(
        bool activeOnly = false)
    {
        IQueryable<CertificateType> query =
            _context.CertificateTypes
                .AsNoTracking();

        if (activeOnly)
        {
            query = query.Where(x => x.IsActive);
        }

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync();
    }


    public async Task<CertificateType?> GetCertificateTypeByIdAsync(
        int id,
        bool tracking = false)
    {
        IQueryable<CertificateType> query =
            _context.CertificateTypes;

        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return await query
            .FirstOrDefaultAsync(
                x => x.CertificateTypeId == id);
    }


    public Task<bool> CertificateTypeNameExistsAsync(
        string name,
        int? excludeId = null)
    {
        var normalizedName = name.Trim();

        return _context.CertificateTypes
            .AnyAsync(x =>
                x.Name == normalizedName &&
                (!excludeId.HasValue ||
                 x.CertificateTypeId != excludeId.Value));
    }


    public Task<bool> CertificateTypeHasRequestsAsync(
        int certificateTypeId)
    {
        return _context.CertificateRequests
            .AnyAsync(x =>
                x.CertificateTypeId == certificateTypeId);
    }


    public async Task<CertificateType> CreateCertificateTypeAsync(
        CertificateType certificateType)
    {
        await _context.CertificateTypes
            .AddAsync(certificateType);

        await _context.SaveChangesAsync();

        return certificateType;
    }


    public async Task<CertificateType> UpdateCertificateTypeAsync(
        CertificateType certificateType)
    {
        _context.CertificateTypes
            .Update(certificateType);

        await _context.SaveChangesAsync();

        return certificateType;
    }


    public async Task DeleteCertificateTypeAsync(
        CertificateType certificateType)
    {
        _context.CertificateTypes
            .Remove(certificateType);

        await _context.SaveChangesAsync();
    }


    // =========================================================
    // CERTIFICATE REQUESTS
    // =========================================================

    public async Task<List<CertificateRequest>> GetAllRequestsAsync(
        CertificateRequestStatus? status,
        int page = 1)
    {
        if (page < 1)
        {
            page = 1;
        }

        IQueryable<CertificateRequest> query =
            _context.CertificateRequests
                .AsNoTracking()
                .Include(x => x.CertificateType)
                .Include(x => x.Student);

        if (status.HasValue)
        {
            query = query.Where(
                x => x.Status == status.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }


    public async Task<CertificateRequest?> GetRequestByIdAsync(
        int id,
        bool tracking = false)
    {
        IQueryable<CertificateRequest> query =
            _context.CertificateRequests
                .Include(x => x.CertificateType)
                .Include(x => x.Student);

        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return await query
            .FirstOrDefaultAsync(
                x => x.CertificateRequestId == id);
    }


    public async Task<List<CertificateRequest>>
        GetRequestsByStudentIdAsync(int studentId)
    {
        return await _context.CertificateRequests
            .AsNoTracking()
            .Include(x => x.CertificateType)
            .Include(x => x.Student)
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }


    public Task<bool> HasPendingRequestAsync(
        int studentId,
        int certificateTypeId)
    {
        return _context.CertificateRequests
            .AnyAsync(x =>
                x.StudentId == studentId &&
                x.CertificateTypeId == certificateTypeId &&
                x.Status == CertificateRequestStatus.Pending);
    }


    public async Task<CertificateRequest> CreateRequestAsync(
        CertificateRequest request)
    {
        await _context.CertificateRequests
            .AddAsync(request);

        await _context.SaveChangesAsync();

        return request;
    }


    public async Task<CertificateRequest> UpdateRequestAsync(
        CertificateRequest request)
    {
        _context.CertificateRequests
            .Update(request);

        await _context.SaveChangesAsync();

        return request;
    }
}