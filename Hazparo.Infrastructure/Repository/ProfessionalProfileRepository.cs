using Hazparo.Domain.Entities;
using Hazparo.Domain.Repositories;
using Hazparo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hazparo.Infrastructure.Persistence.Repositories;

public sealed class ProfessionalProfileRepository(HazparoDbContext dbContext): IProfessionalProfileRepository
{
    public Task<ProfessionalProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return dbContext.ProfessionalProfiles.Include(profile => profile.Location)
        .Include(profile => profile.Services)
            .FirstOrDefaultAsync(
                profile => profile.UserId == userId,
                cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}