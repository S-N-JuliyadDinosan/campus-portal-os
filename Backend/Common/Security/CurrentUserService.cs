using System.Security.Claims;

namespace CampusServicesPortal.Common.Security;

public sealed class CurrentUserService(IHttpContextAccessor accessor)
    : ICurrentUserService
{
    private ClaimsPrincipal? User => accessor.HttpContext?.User;

    public int? UserId => ParseIntClaim(ClaimTypes.NameIdentifier, "userId");
    public int? StudentId => ParseIntClaim("studentId");
    public string? Role => User?.FindFirstValue(ClaimTypes.Role);
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    private int? ParseIntClaim(params string[] claimTypes)
    {
        foreach (var claimType in claimTypes)
        {
            var value = User?.FindFirstValue(claimType);
            if (int.TryParse(value, out var parsed))
            {
                return parsed;
            }
        }

        return null;
    }
}
