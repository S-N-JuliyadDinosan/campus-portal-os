namespace CampusService.Modules.Fees.DTOs;

public class FeeAssignDto
{
    public int? StudentId { get; set; }

    public int? FacultyId { get; set; }

    public int FeeTypeId { get; set; }

    public decimal Amount { get; set; }

    public string BillingPeriod { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }
}