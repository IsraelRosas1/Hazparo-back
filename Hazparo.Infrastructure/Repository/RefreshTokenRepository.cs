using Hazparo.Domain.Entities;
using Hazparo.Domain.Repositories;
using Hazparo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hazparo.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository(
    HazparoDbContext dbContext) : IRefreshTokenRepository
{
    public async Task AddAsync(AuthRefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await dbContext.AuthRefreshTokens.AddAsync(
            refreshToken,
            cancellationToken);
    }

    public Task<AuthRefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return dbContext.AuthRefreshTokens
            .SingleOrDefaultAsync(
                token => token.TokenHash == tokenHash,
                cancellationToken);
    }

    public Task RevokeAllActiveAsync(Guid userId, DateTimeOffset revokedAt, CancellationToken cancellationToken)
    {
        return dbContext.AuthRefreshTokens
            .Where(token =>
                token.UserId == userId &&
                token.RevokedAt == null)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    token => token.RevokedAt,
                    revokedAt),
                cancellationToken);
    }

    public async Task SaveChangesAsync( CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}