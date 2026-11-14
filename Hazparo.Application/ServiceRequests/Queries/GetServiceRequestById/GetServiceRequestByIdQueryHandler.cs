using Hazparo.Application.ServiceRequests.Dtos;
using Hazparo.Application.User;
using Hazparo.Domain.Repositories;
using MediatR;

namespace Hazparo.Application.ServiceRequests.Queries.GetServiceRequestById;

public sealed class GetServiceRequestByIdQueryHandler(
    IUserContext userContext,IServiceRequestRepository serviceRequestRepository)
    : IRequestHandler<GetServiceRequestByIdQuery,ServiceRequestDetailsDto?>
{
    public async Task<ServiceRequestDetailsDto?> Handle(GetServiceRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (currentUser is null ||
            !Guid.TryParse(currentUser.Id, out var clientId))
        {
            return null;
        }

        var serviceRequest =
            await serviceRequestRepository.GetByIdForClientAsync(
                request.RequestId,
                clientId,
                cancellationToken);

        if (serviceRequest is null)
        {
            return null;
        }

        return new ServiceRequestDetailsDto
        {
            Id = serviceRequest.Id,
            CategoryId = serviceRequest.CategoryId,
            CategoryName = serviceRequest.Category.Name,
            Title = serviceRequest.Title,
            Description = serviceRequest.Description,
            IsUrgent = serviceRequest.IsUrgent,
            ScheduledFor = serviceRequest.ScheduledFor,
            Status = serviceRequest.Status,

            AddressLine1 = serviceRequest.AddressLine1,
            AddressLine2 = serviceRequest.AddressLine2,
            Neighborhood = serviceRequest.Neighborhood,
            City = serviceRequest.City,
            State = serviceRequest.State,
            PostalCode = serviceRequest.PostalCode,
            CountryCode = serviceRequest.CountryCode,
            LocationReference = serviceRequest.LocationReference,

            Latitude = serviceRequest.Latitude,
            Longitude = serviceRequest.Longitude,

            CreatedAt = serviceRequest.CreatedAt,
            UpdatedAt = serviceRequest.UpdatedAt,

            Photos = serviceRequest.Photos
                .OrderBy(photo => photo.SortOrder)
                .Select(photo => new ServiceRequestPhotoDto
                {
                    Id = photo.Id,
                    ContentType = photo.ContentType,
                    SortOrder = photo.SortOrder
                })
                .ToList()
        };
    }
}