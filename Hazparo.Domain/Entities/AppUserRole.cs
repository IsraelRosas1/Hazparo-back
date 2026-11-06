namespace Hazparo.Domain.Entities;

public class AppUserRole
{
    public Guid UserId { get; set; }
    public UserRole Role { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public AppUser User { get; set; } = null!;
}
