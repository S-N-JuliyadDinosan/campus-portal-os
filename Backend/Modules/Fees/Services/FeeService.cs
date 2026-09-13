using System.Data;
using CampusService.Modules.Fees.DTOs;
using CampusService.Modules.Fees.Interfaces;
using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Common.Exceptions;
using CampusServicesPortal.Data;
using Microsoft.EntityFrameworkCore;
using CampusServicesPortal.Modules.Fees.Entities;

namespace CampusService.Modules.Fees.Services;

public class FeeService(
    IFeeRepository fees,
    ApplicationDbContext dbContext)
    : IFeeService
{
    private readonly IFeeRepository _fees = fees;
    private readonly ApplicationDbContext _dbContext = dbContext;


    // ============================================
    // FEE TYPES
    // ============================================

    public async Task<List<FeeType>>
        GetFeeTypesAsync()
    {
        return await _fees.GetFeeTypesAsync();
    }


    public async Task<FeeType?>
        GetFeeTypeByIdAsync(
            int id)
    {
        return await _fees.GetFeeTypeByIdAsync(id);
    }


    public async Task<FeeType>
        CreateFeeTypeAsync(
            FeeTypeCreateDto dto)
    {
        var feeType = new FeeType
        {
            Name = dto.Name.Trim(),
            Description = dto.Description
        };

        await _fees.AddFeeTypeAsync(feeType);

        await _fees.SaveChangesAsync();

        return feeType;
    }


    public async Task<bool>
        UpdateFeeTypeAsync(
            int id,
            FeeTypeUpdateDto dto)
    {
        var feeType =
            await _fees.GetFeeTypeByIdAsync(id);

        if (feeType == null)
        {
            return false;
        }

        feeType.Name =
            dto.Name.Trim();

        feeType.Description =
            dto.Description;

        feeType.IsActive =
            dto.IsActive;

        await _fees.UpdateFeeTypeAsync(
            feeType);

        await _fees.SaveChangesAsync();

        return true;
    }


    public async Task<bool>
        DeleteFeeTypeAsync(
            int id)
    {
        var feeType =
            await _fees.GetFeeTypeByIdAsync(id);

        if (feeType == null)
        {
            return false;
        }

        await _fees.DeleteFeeTypeAsync(
            feeType);

        await _fees.SaveChangesAsync();

        return true;
    }


    // ============================================
    // FEE PAYMENTS
    // ============================================

    public async Task<List<FeePayment>>
        GetAllFeesAsync()
    {
        return await _fees.GetAllAsync();
    }


    public async Task<FeePayment?>
        GetFeeByIdAsync(
            int id)
    {
        return await _fees.GetByIdAsync(id);
    }


    public async Task<List<FeePayment>>
        StudentFeesAsync(
            int studentId)
    {
        return await _fees
            .GetStudentFeesAsync(studentId);
    }


    public async Task<List<FeePayment>>
        GetUnpaidFeesAsync(
            int studentId)
    {
        return await _fees
            .GetUnpaidFeesAsync(studentId);
    }


    public async Task<List<FeePayment>>
        GetPaidFeesAsync(
            int studentId)
    {
        return await _fees
            .GetPaidFeesAsync(studentId);
    }


    // ============================================
    // ASSIGN FEE
    // ============================================

    public async Task<bool>
        AssignFeeAsync(
            FeeAssignDto dto,
            int? assignedByUserId)
    {
        // Student OR Faculty must be selected.
        if (dto.StudentId == null &&
            dto.FacultyId == null)
        {
            return false;
        }

        // Both cannot be selected.
        if (dto.StudentId != null &&
            dto.FacultyId != null)
        {
            return false;
        }

        if (dto.Amount <= 0)
        {
            return false;
        }

        var feeTypeExists = await _dbContext.FeeTypes.AnyAsync(x =>
            x.FeeTypeId == dto.FeeTypeId && x.IsActive);

        if (!feeTypeExists)
        {
            throw new BusinessRuleException(
                "Selected fee type does not exist or is inactive.");
        }


        // ----------------------------------------
        // SINGLE STUDENT
        // ----------------------------------------

        if (dto.StudentId.HasValue)
        {
            var studentExists = await _dbContext.Students
                .Include(x => x.User)
                .AnyAsync(x =>
                    x.StudentId == dto.StudentId.Value &&
                    x.User.IsActive);

            if (!studentExists)
            {
                throw new BusinessRuleException(
                    "Selected student does not exist or is inactive.");
            }

            var fee = new FeePayment
            {
                StudentId =
                    dto.StudentId.Value,

                FeeTypeId =
                    dto.FeeTypeId,

                AssignedByUserId =
                    assignedByUserId,

                Amount =
                    dto.Amount,

                BillingPeriod =
                    dto.BillingPeriod.Trim(),

                Status =
                    FeePaymentStatus.Outstanding,

                DueDate =
                    dto.DueDate,

                AssignedAt =
                    DateTime.UtcNow
            };

            await _fees.AddAsync(fee);

            await _fees.SaveChangesAsync();

            return true;
        }


        // ----------------------------------------
        // FACULTY
        // ----------------------------------------

        if (dto.FacultyId.HasValue)
        {
            var studentIds = await _dbContext.Students
                .Include(x => x.User)
                .Where(x =>
                    x.FacultyId == dto.FacultyId.Value &&
                    x.User.IsActive)
                .Select(x => x.StudentId)
                .ToListAsync();

            if (studentIds.Count == 0)
            {
                throw new BusinessRuleException(
                    "No active students were found for the selected faculty.");
            }

            var assignedAt = DateTime.UtcNow;
            var feesForFaculty = studentIds.Select(studentId =>
                new FeePayment
                {
                    StudentId = studentId,
                    FeeTypeId = dto.FeeTypeId,
                    AssignedByUserId = assignedByUserId,
                    Amount = dto.Amount,
                    BillingPeriod = dto.BillingPeriod.Trim(),
                    Status = FeePaymentStatus.Outstanding,
                    DueDate = dto.DueDate,
                    AssignedAt = assignedAt
                }).ToList();

            await _fees.AddRangeAsync(feesForFaculty);
            await _fees.SaveChangesAsync();
            return true;
        }


        return false;
    }


    // ============================================
    // FAKE / DEMO PAYMENT
    // ============================================

    public async Task<FeePayment?>
        SimulatePaymentAsync(
            int id,
            int studentId)
    {
        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        try
        {
            var fee =
                await _fees.GetByIdAsync(id);

            if (fee == null)
            {
                await transaction.RollbackAsync();
                return null;
            }

            // Student can only pay their own fee.
            if (fee.StudentId != studentId)
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to pay this fee.");
            }

            // BRD rule: a fee cannot be paid more than once.
            if (fee.Status != FeePaymentStatus.Outstanding)
            {
                throw new BusinessRuleException(
                    "This fee has already been paid or is not payable.");
            }

            var now = DateTime.UtcNow;

            fee.Status = FeePaymentStatus.Paid;
            fee.PaidAt = now;
            fee.PaymentMethod = "Demo Card";
            fee.PaymentReference =
                $"DEMO-{Guid.NewGuid():N}".ToUpperInvariant();
            fee.ReceiptNumber =
                $"REC-{now:yyyyMMddHHmmssfff}-{fee.FeePaymentId}";

            await _fees.UpdateAsync(fee);
            await _fees.SaveChangesAsync();
            await transaction.CommitAsync();

            return fee;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    // ============================================
    // UPDATE PAYMENT STATUS
    // ============================================

    public async Task<bool>
        UpdatePaymentStatusAsync(
            int id,
            FeePaymentUpdateDto dto)
    {
        var fee =
            await _fees.GetByIdAsync(id);

        if (fee == null)
        {
            return false;
        }


        // ----------------------------------------
        // Convert string to enum
        // ----------------------------------------

        if (!Enum.TryParse<FeePaymentStatus>(
                dto.Status,
                true,
                out var status))
        {
            return false;
        }


        fee.Status =
            status;


        // ----------------------------------------
        // PAID
        // ----------------------------------------

        if (status ==
            FeePaymentStatus.Paid)
        {
            var now =
                DateTime.UtcNow;

            fee.PaidAt ??=
                now;

            fee.PaymentMethod ??=
                "Admin Update";

            fee.PaymentReference ??=
                $"ADMIN-{Guid.NewGuid():N}"
                    .ToUpperInvariant();

            fee.ReceiptNumber ??=
                $"REC-{now:yyyyMMddHHmmssfff}-{fee.FeePaymentId}";
        }


        await _fees.UpdateAsync(fee);

        await _fees.SaveChangesAsync();

        return true;
    }


    // ============================================
    // DELETE FEE
    // ============================================

    public async Task<bool>
        DeleteFeeAsync(
            int id)
    {
        var fee =
            await _fees.GetByIdAsync(id);

        if (fee == null)
        {
            return false;
        }

        await _fees.DeleteAsync(fee);

        await _fees.SaveChangesAsync();

        return true;
    }
}
