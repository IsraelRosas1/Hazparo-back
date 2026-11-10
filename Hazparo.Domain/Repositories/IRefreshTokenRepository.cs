using Hazparo.Domain.Entities;

namespace Hazparo.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync( AuthRefreshToken refreshToken,CancellationToken cancellationToken);

    Task<AuthRefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken);

    Task RevokeAllActiveAsync(Guid userId,DateTimeOffset revokedAt, CancellationToken cancellationToken);

    Task SaveChangesAsync( CancellationToken cancellationToken);
}