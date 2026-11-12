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
    public async Task<IReadOnlyList<Guid>>
    GetActiveCategoryIdsAsync(
        IReadOnlyCollection<Guid> categoryIds, CancellationToken cancellationToken)
    {
        return await dbContext.ServiceCategories
            .Where(category =>
                category.IsActive &&
                categoryIds.Contains(category.Id))
            .Select(category => category.Id)
            .ToListAsync(cancellationToken);
    }
}