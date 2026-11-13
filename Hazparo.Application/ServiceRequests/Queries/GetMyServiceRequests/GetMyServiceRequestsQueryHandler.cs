using Hazparo.Application.ServiceRequests.Dtos;
using Hazparo.Application.User;
using Hazparo.Domain.Repositories;
using MediatR;

namespace Hazparo.Application.ServiceRequests.Queries
    .GetMyServiceRequests;

public sealed class GetMyServiceRequestsQueryHandler(
    IUserContext userContext,
    IServiceRequestRepository serviceRequestRepository)
    : IRequestHandler<
        GetMyServiceRequestsQuery,
        List<ServiceRequestSummaryDto>>
{
    public async Task<List<ServiceRequestSummaryDto>> Handle(GetMyServiceRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (currentUser is null ||
            !Guid.TryParse(currentUser.Id, out var clientId))
        {
            return [];
        }

        var requests =
            await serviceRequestRepository.GetByClientIdAsync(
                clientId,
                cancellationToken);

        return requests.Select(serviceRequest =>
            new ServiceRequestSummaryDto
            {
                Id = serviceRequest.Id,
                CategoryId = serviceRequest.CategoryId,
                CategoryName = serviceRequest.Category.Name,
                Title = serviceRequest.Title,
                Description = serviceRequest.Description,
                IsUrgent = serviceRequest.IsUrgent,
                Status = serviceRequest.Status,
                ScheduledFor = serviceRequest.ScheduledFor,
                AddressLine1 = serviceRequest.AddressLine1,
                City = serviceRequest.City,
                LocationReference = serviceRequest.LocationReference,
                Latitude = serviceRequest.Latitude,
                Longitude = serviceRequest.Longitude,
                CreatedAt = serviceRequest.CreatedAt,
                Photos = serviceRequest.Photos
                    .OrderBy(photo => photo.SortOrder)
                    .Select(photo => new ServiceRequestPhotoDto
                    {
                        Id = photo.Id,
                        ContentType = photo.ContentType,
                        SortOrder = photo.SortOrder
                    })
                    .ToList()
            })
            .ToList();
    }
}