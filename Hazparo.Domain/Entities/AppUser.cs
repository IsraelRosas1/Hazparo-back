namespace Hazparo.Domain.Entities;

public class AppUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string? NormalizedEmail { get; private set; }
    public string PasswordHash { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? AvatarObjectKey { get; set; }
    public AccountStatus Status { get; set; } = AccountStatus.Pending;
    public DateTimeOffset? EmailConfirmedAt { get; set; }
    public DateTimeOffset? PhoneConfirmedAt { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<AppUserRole> Roles { get; set; } = new List<AppUserRole>();
    public ClientProfile? ClientProfile { get; set; }
    public ProfessionalProfile? ProfessionalProfile { get; set; }
}
