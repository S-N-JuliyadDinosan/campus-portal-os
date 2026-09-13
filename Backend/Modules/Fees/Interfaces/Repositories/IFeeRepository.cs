using CampusServicesPortal.Modules.Fees.Entities;

namespace CampusService.Modules.Fees.Interfaces;

public interface IFeeRepository
{
    IQueryable<FeePayment> Query();

    Task<FeePayment?> GetByIdAsync(int id);

    Task<List<FeePayment>> GetStudentFeesAsync(int studentId);

    Task<List<FeePayment>> GetUnpaidFeesAsync(int studentId);

    Task<List<FeePayment>> GetPaidFeesAsync(int studentId);

    Task<List<FeePayment>> GetAllAsync();

    Task AddAsync(FeePayment fee);

    Task AddRangeAsync(IEnumerable<FeePayment> fees);

    Task UpdateAsync(FeePayment fee);

    Task DeleteAsync(FeePayment fee);

    Task SaveChangesAsync();

    // Fee Types
    Task<List<FeeType>> GetFeeTypesAsync();

    Task<FeeType?> GetFeeTypeByIdAsync(int id);

    Task AddFeeTypeAsync(FeeType feeType);

    Task UpdateFeeTypeAsync(FeeType feeType);

    Task DeleteFeeTypeAsync(FeeType feeType);
}



