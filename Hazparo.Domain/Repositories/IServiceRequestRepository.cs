using Hazparo.Domain.Entities;

namespace Hazparo.Domain.Repositories;

public interface IServiceRequestRepository
{
    Task<bool> ClientProfileExistsAsync(Guid clientId, CancellationToken cancellationToken);

    Task<bool> CategoryIsActiveAsync(Guid categoryId, CancellationToken cancellationToken);

    Task CreateAsync(ServiceRequest request, IReadOnlyCollection<ServiceRequestPhoto> photos, CancellationToken cancellationToken);

    Task<IReadOnlyList<ServiceRequest>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken);
    Task<ServiceRequest?> GetByIdForClientAsync(Guid requestId, Guid clientId, CancellationToken cancellationToken);

    Task<ServiceRequestPhoto?> GetPhotoByIdForClientAsync(Guid requestId,Guid photoId,Guid clientId, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}