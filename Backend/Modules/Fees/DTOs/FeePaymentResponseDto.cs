namespace CampusService.Modules.Fees.DTOs;

public class FeePaymentResponseDto

{
    public int FeePaymentId { get; set; }

    public int StudentId { get; set; }

    public int FeeTypeId { get; set; }

    public string FeeTypeName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string BillingPeriod { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }

    public string? ReceiptNumber { get; set; }
}