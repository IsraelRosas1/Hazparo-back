using Microsoft.AspNetCore.Identity;

namespace Hazparo.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = null!;

    public AccountStatus Status { get; set; } = AccountStatus.Pending;

    public DateTimeOffset? EmailConfirmedAt { get; set; }
    public DateTimeOffset? PhoneConfirmedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public ClientProfile? ClientProfile { get; set; }
    public ProfessionalProfile? ProfessionalProfile { get; set; }
    public ICollection<AuthRefreshToken> RefreshTokens { get; set; } = new List<AuthRefreshToken>();
}