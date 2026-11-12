using Hazparo.Domain.Entities;
using Hazparo.Domain.Repositories;
using Hazparo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hazparo.Infrastructure.Persistence.Repositories;

public sealed class ServiceCategoryRepository(
    HazparoDbContext dbContext)
    : IServiceCategoryRepository
{
    public async Task<IReadOnlyList<ServiceCategory>> GetActiveAsync( CancellationToken cancellationToken)
    {
        return await dbContext.ServiceCategories
            .AsNoTracking()
            .Where(category => category.IsActive)
            .OrderBy(category => category.SortOrder)
            .ToListAsync(cancellationToken);
    }
}