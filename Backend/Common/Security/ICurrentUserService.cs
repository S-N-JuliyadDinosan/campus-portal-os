namespace CampusServicesPortal.Common.Security;

public interface ICurrentUserService
{
    int? UserId { get; }
    int? StudentId { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}
