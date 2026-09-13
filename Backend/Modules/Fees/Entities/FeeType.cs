using CampusServicesPortal.Common.Entities;
using System.Text.Json.Serialization;

namespace CampusServicesPortal.Modules.Fees.Entities;

public sealed class FeeType : AuditableEntity
{
    public int FeeTypeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    [JsonIgnore]
    public ICollection<FeePayment> FeePayments { get; set; } = [];
}