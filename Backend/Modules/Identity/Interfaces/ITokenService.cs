using CampusServicesPortal.Modules.Identity.Entities;

namespace CampusServicesPortal.Modules.Identity.Interfaces;

public interface ITokenService
{
    string CreateAccessToken(User user, int? studentId);
    string CreateRefreshToken();
    string CreateOneTimeToken();
    string HashToken(string rawToken);
}
