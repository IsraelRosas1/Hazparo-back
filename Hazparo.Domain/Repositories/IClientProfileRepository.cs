using Hazparo.Domain.Entities;

namespace Hazparo.Domain.Repositories;

public interface IClientProfileRepository
{
    Task AddAsync(ClientProfile clientProfile, CancellationToken cancellationToken);

    Task SaveChangesAsync( CancellationToken cancellationToken);
}