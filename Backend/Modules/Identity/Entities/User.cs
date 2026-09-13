using CampusServicesPortal.Common.Entities;
using CampusServicesPortal.Modules.Students.Entities;

namespace CampusServicesPortal.Modules.Identity.Entities;

public sealed class User : AuditableEntity
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Student";
    public bool EmailVerified { get; set; }
    public bool IsActive { get; set; } = true;
    public string SecurityStamp { get; set; } = Guid.NewGuid().ToString("N");

    public Student? Student { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = [];
    public ICollection<EmailVerificationToken> EmailVerificationTokens { get; set; } = [];
}
