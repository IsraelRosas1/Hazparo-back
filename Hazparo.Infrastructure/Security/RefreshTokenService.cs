using System.Net;
using System.Security.Cryptography;
using System.Text;
using Hazparo.Domain.Entities;
using Hazparo.Domain.Interfaces;
using Hazparo.Domain.Models;
using Hazparo.Domain.Repositories;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace Hazparo.Infrastructure.Security;

public sealed class RefreshTokenService(
    IRefreshTokenRepository refreshTokenRepository,
    IConfiguration configuration) : IRefreshTokenService
{
    public async Task<IssuedRefreshToken> IssueAsync(
        Guid userId,
        string? userAgent,
        IPAddress? ipAddress,
        CancellationToken cancellationToken)
    {
        var rawToken = CreateRawToken();
        var now = DateTimeOffset.UtcNow;

        var expiresAt = now.AddDays(
            configuration.GetValue<int?>(
                "Jwt:RefreshTokenDays") ?? 30);

        var refreshToken = new AuthRefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = Hash(rawToken),
            ExpiresAt = expiresAt,
            UserAgent = userAgent,
            IpAddress = ipAddress,
            CreatedAt = now
        };

        await refreshTokenRepository.AddAsync(
            refreshToken,
            cancellationToken);

        await refreshTokenRepository.SaveChangesAsync(
            cancellationToken);

        return new IssuedRefreshToken
        {
            Token = rawToken,
            ExpiresAt = expiresAt
        };
    }

    public async Task<RotatedRefreshToken?> RotateAsync(
        string rawRefreshToken,
        string? userAgent,
        IPAddress? ipAddress,
        CancellationToken cancellationToken)
    {
        var currentToken =
            await refreshTokenRepository.GetByHashAsync(
                Hash(rawRefreshToken),
                cancellationToken);

        if (currentToken is null)
        {
            return null;
        }

        var now = DateTimeOffset.UtcNow;

        if (currentToken.RevokedAt is not null)
        {
            await refreshTokenRepository.RevokeAllActiveAsync(
                currentToken.UserId,
                now,
                cancellationToken);

            return null;
        }

        if (currentToken.ExpiresAt <= now)
        {
            return null;
        }

        var replacementRawToken = CreateRawToken();

        var replacementExpiresAt = now.AddDays(
            configuration.GetValue<int?>(
                "Jwt:RefreshTokenDays") ?? 30);

        var replacementToken = new AuthRefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = currentToken.UserId,
            TokenHash = Hash(replacementRawToken),
            ExpiresAt = replacementExpiresAt,
            UserAgent = userAgent,
            IpAddress = ipAddress,
            CreatedAt = now
        };

        currentToken.RevokedAt = now;
        currentToken.ReplacedByTokenId = replacementToken.Id;

        await refreshTokenRepository.AddAsync(
            replacementToken,
            cancellationToken);

        await refreshTokenRepository.SaveChangesAsync(
            cancellationToken);

        return new RotatedRefreshToken
        {
            UserId = currentToken.UserId,
            Token = replacementRawToken,
            ExpiresAt = replacementExpiresAt
        };
    }

    public async Task RevokeAllAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        await refreshTokenRepository.RevokeAllActiveAsync(
            userId,
            DateTimeOffset.UtcNow,
            cancellationToken);
    }

    private static string CreateRawToken()
    {
        return WebEncoders.Base64UrlEncode(
            RandomNumberGenerator.GetBytes(64));
    }

    private static string Hash(string rawToken)
    {
        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(rawToken));

        return Convert.ToHexString(bytes)
            .ToLowerInvariant();
    }
}