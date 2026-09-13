namespace CampusService.Modules.Fees.DTOs;

public sealed record FeeReceiptDto(
    int FeePaymentId,
    int StudentId,
    string FeeType,
    string BillingPeriod,
    decimal Amount,
    string ReceiptNumber,
    DateTime PaidAt,
    string PaymentMethod,
    string PaymentReference);
