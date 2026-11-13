using Hazparo.Application.User;
using Hazparo.Domain.Entities;
using Hazparo.Domain.Interfaces;
using Hazparo.Domain.Repositories;
using MediatR;
using NetTopologySuite.Geometries;

namespace Hazparo.Application.ServiceRequests.Commands
    .CreateServiceRequest;

public sealed class CreateServiceRequestCommandHandler(
    IUserContext userContext,
    IServiceRequestRepository serviceRequestRepository,
    IPrivateFileStorage privateFileStorage)
    : IRequestHandler<CreateServiceRequestCommand, Guid?>
{
    private const long MaxPhotoSize = 5 * 1024 * 1024;

    private static readonly HashSet<string>
        AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/jpg",
            "image/png",
            "image/heic",
            "image/heif",
            "image/webp",
            "image/avif"
        };

    public async Task<Guid?> Handle(CreateServiceRequestCommand request,
        CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (currentUser is null ||
            !Guid.TryParse(currentUser.Id, out var clientId))
        {
            return null;
        }

        var clientExists =
            await serviceRequestRepository.ClientProfileExistsAsync(
                clientId,
                cancellationToken);

        if (!clientExists)
        {
            return null;
        }

        var categoryIsActive =
            await serviceRequestRepository.CategoryIsActiveAsync(
                request.CategoryId,
                cancellationToken);

        if (!categoryIsActive)
        {
            return null;
        }

        foreach (var photo in request.Photos)
        {
            if (photo.Length <= 0 ||
                photo.Length > MaxPhotoSize)
            {
                return null;
            }

            if (!AllowedContentTypes.Contains(photo.ContentType))
            {
                return null;
            }
        }

        var now = DateTimeOffset.UtcNow;
        var requestId = Guid.NewGuid();

        var location = new Point(
            (double)request.Longitude,
            (double)request.Latitude)
        {
            SRID = 4326
        };

        var serviceRequest = new ServiceRequest
        {
            Id = requestId,
            ClientId = clientId,
            CategoryId = request.CategoryId,
            Title = request.Title?.Trim(),
            Description = request.Description.Trim(),
            IsUrgent = request.IsUrgent,
            ScheduledFor = request.ScheduledFor,
            Status = ServiceRequestStatus.Open,

            AddressLine1 = request.AddressLine1.Trim(),
            AddressLine2 = request.AddressLine2?.Trim(),
            Neighborhood = request.Neighborhood?.Trim(),
            City = request.City.Trim(),
            State = request.State?.Trim(),
            PostalCode = request.PostalCode?.Trim(),
            CountryCode = request.CountryCode.Trim()
                .ToUpperInvariant(),
            LocationReference = request.LocationReference?.Trim(),

            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Location = location,

            CreatedAt = now,
            UpdatedAt = now
        };

        var savedKeys = new List<string>();
        var photos = new List<ServiceRequestPhoto>();

        try
        {
            for (var index = 0;
                 index < request.Photos.Count;
                 index++)
            {
                var uploadedPhoto = request.Photos[index];

                var objectKey =
                    await privateFileStorage
                        .SaveServiceRequestPhotoAsync(
                            uploadedPhoto.Content,
                            requestId,
                            cancellationToken);

                savedKeys.Add(objectKey);

                photos.Add(new ServiceRequestPhoto
                {
                    Id = Guid.NewGuid(),
                    RequestId = requestId,
                    ObjectKey = objectKey,
                    ContentType = uploadedPhoto.ContentType,
                    SortOrder = (short)index,
                    CreatedAt = now
                });
            }

            await serviceRequestRepository.CreateAsync(
                serviceRequest,
                photos,
                cancellationToken);

            return requestId;
        }
        catch
        {
            foreach (var objectKey in savedKeys)
            {
                await privateFileStorage.DeleteAsync(
                    objectKey,
                    cancellationToken);
            }

            throw;
        }
    }
}