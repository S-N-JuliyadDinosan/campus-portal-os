using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Fees.Entities;
using CampusService.Modules.Fees.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusService.Modules.Fees.Repositories;

public class FeeRepository(ApplicationDbContext context) : IFeeRepository
{
    private readonly ApplicationDbContext _context = context;

    // ==========================================
    // FEE PAYMENTS
    // ==========================================

    public IQueryable<FeePayment> Query()
    {
        return _context.FeePayments
            .Include(x => x.FeeType)
            .AsQueryable();
    }

    public async Task<FeePayment?> GetByIdAsync(int id)
    {
        return await _context.FeePayments
            .Include(x => x.FeeType)
            .FirstOrDefaultAsync(x => x.FeePaymentId == id);
    }

    public async Task<List<FeePayment>> GetStudentFeesAsync(int studentId)
    {
        return await _context.FeePayments
            .Include(x => x.FeeType)
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.DueDate)
            .ToListAsync();
    }

    public async Task<List<FeePayment>> GetUnpaidFeesAsync(int studentId)
    {
        return await _context.FeePayments
            .Include(x => x.FeeType)
            .Where(x =>
                x.StudentId == studentId &&
                x.Status == FeePaymentStatus.Outstanding)
            .OrderBy(x => x.DueDate)
            .ToListAsync();
    }

    public async Task<List<FeePayment>> GetPaidFeesAsync(int studentId)
    {
        return await _context.FeePayments
            .Include(x => x.FeeType)
            .Where(x =>
                x.StudentId == studentId &&
                x.Status == FeePaymentStatus.Paid)
            .OrderByDescending(x => x.DueDate)
            .ToListAsync();
    }

    public async Task<List<FeePayment>> GetAllAsync()
    {
        return await _context.FeePayments
            .Include(x => x.FeeType)
            .Include(x => x.Student)
            .OrderByDescending(x => x.FeePaymentId)
            .ToListAsync();
    }

    public async Task AddAsync(FeePayment fee)
    {
        await _context.FeePayments.AddAsync(fee);
    }

    public async Task AddRangeAsync(IEnumerable<FeePayment> fees)
    {
        await _context.FeePayments.AddRangeAsync(fees);
    }

    public Task UpdateAsync(FeePayment fee)
    {
        _context.FeePayments.Update(fee);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(FeePayment fee)
    {
        _context.FeePayments.Remove(fee);

        return Task.CompletedTask;
    }

    // ==========================================
    // FEE TYPES
    // ==========================================

    public async Task<List<FeeType>> GetFeeTypesAsync()
    {
        return await _context.FeeTypes
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<FeeType?> GetFeeTypeByIdAsync(int id)
    {
        return await _context.FeeTypes
            .FirstOrDefaultAsync(x => x.FeeTypeId == id);
    }

    public async Task AddFeeTypeAsync(FeeType feeType)
    {
        await _context.FeeTypes.AddAsync(feeType);
    }

    public Task UpdateFeeTypeAsync(FeeType feeType)
    {
        _context.FeeTypes.Update(feeType);

        return Task.CompletedTask;
    }

    public Task DeleteFeeTypeAsync(FeeType feeType)
    {
        _context.FeeTypes.Remove(feeType);

        return Task.CompletedTask;
    }

    // ==========================================
    // SAVE CHANGES
    // ==========================================

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
