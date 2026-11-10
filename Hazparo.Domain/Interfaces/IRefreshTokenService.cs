using System.Net;
using Hazparo.Domain.Models;

namespace Hazparo.Domain.Interfaces;

public interface IRefreshTokenService
{
    Task<IssuedRefreshToken> IssueAsync(
        Guid userId,
        string? userAgent,
        IPAddress? ipAddress,
        CancellationToken cancellationToken);

    Task<RotatedRefreshToken?> RotateAsync(
        string rawRefreshToken,
        string? userAgent,
        IPAddress? ipAddress,
        CancellationToken cancellationToken);

    Task RevokeAllAsync(
        Guid userId,
        CancellationToken cancellationToken);
}