namespace CampusServicesPortal.Common.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "CampusServicesPortal";
    public string Audience { get; set; } = "CampusServicesPortal.Client";
    public string Key { get; set; } =
        "DEVELOPMENT-ONLY-CHANGE-USING-USER-SECRETS-1234567890";
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 7;
}
