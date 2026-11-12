using Hazparo.Domain.Entities;

namespace Hazparo.Domain.Repositories;

public interface IServiceCategoryRepository
{
    Task<IReadOnlyList<ServiceCategory>> GetActiveAsync( CancellationToken cancellationToken);
    Task<IReadOnlyList<Guid>> GetActiveCategoryIdsAsync(IReadOnlyCollection<Guid> categoryIds, CancellationToken cancellationToken);
}