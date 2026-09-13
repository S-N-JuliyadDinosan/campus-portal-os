using CampusService.Modules.Fees.DTOs;
using CampusService.Modules.Fees.Interfaces;
using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Common.Exceptions;
using CampusServicesPortal.Common.Security;
using CampusServicesPortal.Modules.Fees.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusService.Modules.Fees.Controllers;

[ApiController]
[Route("api/fee-payments")]
[Authorize]
public sealed class FeePaymentsController(
    IFeeService feeService,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<FeePaymentResponseDto>>> GetAll()
    {
        var fees = await feeService.GetAllFeesAsync();
        return Ok(fees.Select(Map));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<ActionResult<FeePaymentResponseDto>> GetById(int id)
    {
        var fee = await feeService.GetFeeByIdAsync(id);
        if (fee is null)
            return NotFound(new { message = "Fee payment not found." });

        if (!CanAccessStudent(fee.StudentId))
            return Forbid();

        return Ok(Map(fee));
    }

    // BRD: view a student's fees and payment history.
    [HttpGet("student/{studentId:int}")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<ActionResult<IEnumerable<FeePaymentResponseDto>>> GetStudentFees(
        int studentId)
    {
        if (!CanAccessStudent(studentId))
            return Forbid();

        var fees = await feeService.StudentFeesAsync(studentId);
        return Ok(fees.Select(Map));
    }

    // BRD: simulation only; no external payment gateway is called.
    [HttpPost("{id:int}/pay")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<FeePaymentResponseDto>> Pay(int id)
    {
        var studentId = currentUser.StudentId
            ?? throw new UnauthorizedAccessException(
                "Student identity was not found in the access token.");

        var fee = await feeService.SimulatePaymentAsync(id, studentId);
        if (fee is null)
            return NotFound(new { message = "Fee payment not found." });

        return Ok(Map(fee));
    }

    // Returns receipt data for the Angular printable receipt component.
    [HttpGet("{id:int}/receipt")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<ActionResult<FeeReceiptDto>> GetReceipt(int id)
    {
        var fee = await feeService.GetFeeByIdAsync(id);
        if (fee is null)
            return NotFound(new { message = "Fee payment not found." });

        if (!CanAccessStudent(fee.StudentId))
            return Forbid();

        if (fee.Status != FeePaymentStatus.Paid ||
            !fee.PaidAt.HasValue ||
            string.IsNullOrWhiteSpace(fee.ReceiptNumber))
        {
            throw new BusinessRuleException(
                "A receipt is only available after the fee has been paid.");
        }

        return Ok(new FeeReceiptDto(
            fee.FeePaymentId,
            fee.StudentId,
            fee.FeeType?.Name ?? string.Empty,
            fee.BillingPeriod,
            fee.Amount,
            fee.ReceiptNumber,
            fee.PaidAt.Value,
            fee.PaymentMethod ?? "Simulation",
            fee.PaymentReference ?? string.Empty));
    }

    private bool CanAccessStudent(int studentId)
    {
        if (User.IsInRole("Admin"))
            return true;

        return currentUser.StudentId.HasValue &&
               currentUser.StudentId.Value == studentId;
    }

    private static FeePaymentResponseDto Map(FeePayment fee) => new()
    {
        FeePaymentId = fee.FeePaymentId,
        StudentId = fee.StudentId,
        FeeTypeId = fee.FeeTypeId,
        FeeTypeName = fee.FeeType?.Name ?? string.Empty,
        Amount = fee.Amount,
        BillingPeriod = fee.BillingPeriod,
        Status = fee.Status.ToString(),
        DueDate = fee.DueDate,
        ReceiptNumber = fee.ReceiptNumber
    };
}
