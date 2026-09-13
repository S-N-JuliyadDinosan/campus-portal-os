using CampusServicesPortal.Common.Entities;
using CampusServicesPortal.Modules.Identity.Entities;

namespace CampusServicesPortal.Modules.SystemSettings.Entities;

public sealed class SystemSetting : AuditableEntity
{
    public int SystemSettingId { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? UpdatedByUserId { get; set; }

    public User? UpdatedByUser { get; set; }
}
