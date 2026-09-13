namespace CampusServicesPortal.Modules.SystemSettings.DTOs;

public sealed class SystemSettingResponseDto
{
    public int SystemSettingId { get; set; }

    public string SettingKey { get; set; } = string.Empty;

    public string SettingValue { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? UpdatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }
}