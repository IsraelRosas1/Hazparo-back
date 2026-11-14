using Hazparo.Application.ServiceRequests.Queries.GetServiceRequestPhoto;
using Hazparo.Application.User;
using Hazparo.Domain.Interfaces;
using Hazparo.Domain.Repositories;
using MediatR;

namespace Hazparo.Application.ServiceRequests.Queries.GetServiceRequestPhoto;

public sealed class GetServiceRequestPhotoQueryHandler(
    IUserContext userContext,
    IServiceRequestRepository serviceRequestRepository,
    IPrivateFileStorage privateFileStorage)
    : IRequestHandler<GetServiceRequestPhotoQuery,ServiceRequestPhotoFileDto?>
{
    public async Task<ServiceRequestPhotoFileDto?> Handle(GetServiceRequestPhotoQuery request,CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (currentUser is null ||
            !Guid.TryParse(currentUser.Id, out var clientId))
        {
            return null;
        }

        var photo =
            await serviceRequestRepository.GetPhotoByIdForClientAsync(
                request.RequestId,
                request.PhotoId,
                clientId,
                cancellationToken);

        if (photo is null)
        {
            return null;
        }

        var stream =
            await privateFileStorage.OpenReadAsync(
                photo.ObjectKey,
                cancellationToken);

        if (stream is null)
        {
            return null;
        }

        return new ServiceRequestPhotoFileDto
        {
            Content = stream,
            ContentType =
                photo.ContentType ?? "application/octet-stream"
        };
    }
}