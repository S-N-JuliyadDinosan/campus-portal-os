using CampusServicesPortal.Common.Entities;
using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Modules.Identity.Entities;
using CampusServicesPortal.Modules.Labs.Entities;
using CampusServicesPortal.Modules.Students.Entities;

namespace CampusServicesPortal.Modules.Fees.Entities;

public sealed class FeePayment : AuditableEntity
{
    public int FeePaymentId { get; set; }
    public int StudentId { get; set; }
    public int FeeTypeId { get; set; }
    public int? AssignedByUserId { get; set; }
    public int? LabBookingId { get; set; }

    public string BillingPeriod { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }

    public FeePaymentStatus Status { get; set; }
        = FeePaymentStatus.Outstanding;

    public DateTime DueDate { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }

    public string? PaymentMethod { get; set; }
    public string? PaymentReference { get; set; }
    public string? ReceiptNumber { get; set; }
    public string? FineReason { get; set; }

    public Student Student { get; set; } = null!;
    public FeeType FeeType { get; set; } = null!;
    public User? AssignedByUser { get; set; }
    public LabBooking? LabBooking { get; set; }
}