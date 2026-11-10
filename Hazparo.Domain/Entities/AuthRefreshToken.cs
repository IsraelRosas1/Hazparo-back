using System.Net;

namespace Hazparo.Domain.Entities;

public class AuthRefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public Guid? ReplacedByTokenId { get; set; }
    public string? UserAgent { get; set; }
    public IPAddress? IpAddress { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public AuthRefreshToken? ReplacedByToken { get; set; }
}