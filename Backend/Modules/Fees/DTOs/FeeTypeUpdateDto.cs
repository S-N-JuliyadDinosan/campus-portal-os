namespace CampusService.Modules.Fees.DTOs;

public class FeeTypeUpdateDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}
