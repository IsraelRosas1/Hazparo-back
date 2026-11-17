using Hazparo.Domain.Entities;

namespace Hazparo.Domain.Repositories;

public interface IProfessionalProfileRepository
{
    Task<ProfessionalProfile?> GetByUserIdAsync(Guid userId,CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}