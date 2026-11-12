using Hazparo.Domain.Entities;
using Hazparo.Domain.Repositories;
using Hazparo.Infrastructure.Persistence;

namespace Hazparo.Infrastructure.Persistence.Repositories;

public sealed class ClientProfileRepository(HazparoDbContext dbContext)
    : IClientProfileRepository
{
    public async Task AddAsync(ClientProfile clientProfile, CancellationToken cancellationToken)
    {
        await dbContext.ClientProfiles.AddAsync(
            clientProfile,
            cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}