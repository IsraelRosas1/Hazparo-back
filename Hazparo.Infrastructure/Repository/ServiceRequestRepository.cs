using Hazparo.Domain.Entities;
using Hazparo.Domain.Repositories;
using Hazparo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hazparo.Infrastructure.Persistence.Repositories;

public sealed class ServiceRequestRepository(
    HazparoDbContext dbContext)
    : IServiceRequestRepository
{
    public Task<bool> ClientProfileExistsAsync(Guid clientId, CancellationToken cancellationToken)
    {
        return dbContext.ClientProfiles.AnyAsync(
            profile => profile.UserId == clientId,
            cancellationToken);
    }

    public Task<bool> CategoryIsActiveAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        return dbContext.ServiceCategories.AnyAsync(
            category =>
                category.Id == categoryId &&
                category.IsActive,
            cancellationToken);
    }

    public async Task CreateAsync(ServiceRequest request, IReadOnlyCollection<ServiceRequestPhoto> photos, CancellationToken cancellationToken)
    {
        await dbContext.ServiceRequests.AddAsync(
            request,
            cancellationToken);

        await dbContext.ServiceRequestPhotos.AddRangeAsync(
            photos,
            cancellationToken);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<IReadOnlyList<ServiceRequest>>GetByClientIdAsync(
            Guid clientId, CancellationToken cancellationToken)
    {
        return await dbContext.ServiceRequests
            .AsNoTracking()
            .Include(request => request.Category)
            .Include(request => request.Photos)
            .Where(request => request.ClientId == clientId)
            .OrderByDescending(request => request.CreatedAt)
            .ToListAsync(cancellationToken);
    }
    public Task<ServiceRequest?> GetByIdForClientAsync(Guid requestId,Guid clientId,CancellationToken cancellationToken)
    {
        return dbContext.ServiceRequests
            .Include(request => request.Category)
            .Include(request => request.Photos)
            .FirstOrDefaultAsync(
                request =>
                    request.Id == requestId &&
                    request.ClientId == clientId,
                cancellationToken);
    }

    public Task<ServiceRequestPhoto?> GetPhotoByIdForClientAsync(Guid requestId,Guid photoId,Guid clientId, CancellationToken cancellationToken)
    {
        return dbContext.ServiceRequestPhotos
            .Include(photo => photo.Request)
            .FirstOrDefaultAsync(
                photo =>
                    photo.Id == photoId &&
                    photo.RequestId == requestId &&
                    photo.Request.ClientId == clientId,
                cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}