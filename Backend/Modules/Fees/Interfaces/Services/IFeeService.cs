using CampusService.Modules.Fees.DTOs;
using CampusServicesPortal.Modules.Fees.Entities;

namespace CampusService.Modules.Fees.Interfaces;

public interface IFeeService
{
    // ============================================
    // FEE TYPES
    // ============================================

    Task<List<FeeType>> GetFeeTypesAsync();

    Task<FeeType?> GetFeeTypeByIdAsync(
        int id);

    Task<FeeType> CreateFeeTypeAsync(
        FeeTypeCreateDto dto);

    Task<bool> UpdateFeeTypeAsync(
        int id,
        FeeTypeUpdateDto dto);

    Task<bool> DeleteFeeTypeAsync(
        int id);


    // ============================================
    // FEE PAYMENTS
    // ============================================

    Task<List<FeePayment>> GetAllFeesAsync();

    Task<FeePayment?> GetFeeByIdAsync(
        int id);

    Task<List<FeePayment>> StudentFeesAsync(
        int studentId);

    Task<List<FeePayment>> GetUnpaidFeesAsync(
        int studentId);

    Task<List<FeePayment>> GetPaidFeesAsync(
        int studentId);


    // ============================================
    // ASSIGN FEE
    // ============================================

    Task<bool> AssignFeeAsync(
        FeeAssignDto dto,
        int? assignedByUserId);


    // ============================================
    // FAKE / DEMO PAYMENT
    // ============================================

    Task<FeePayment?> SimulatePaymentAsync(
        int id,
        int studentId);


    // ============================================
    // UPDATE PAYMENT STATUS
    // ============================================

    Task<bool> UpdatePaymentStatusAsync(
        int id,
        FeePaymentUpdateDto dto);


    // ============================================
    // DELETE FEE
    // ============================================

    Task<bool> DeleteFeeAsync(
        int id);
}