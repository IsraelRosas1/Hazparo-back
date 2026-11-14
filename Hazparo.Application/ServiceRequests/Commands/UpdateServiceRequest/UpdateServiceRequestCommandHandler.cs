using Hazparo.Application.User;
using Hazparo.Domain.Entities;
using Hazparo.Domain.Repositories;
using MediatR;
using NetTopologySuite.Geometries;

namespace Hazparo.Application.ServiceRequests.Commands.UpdateServiceRequest;

public sealed class UpdateServiceRequestCommandHandler(
    IUserContext userContext,
    IServiceRequestRepository serviceRequestRepository)
    : IRequestHandler<UpdateServiceRequestCommand, bool>
{
    public async Task<bool> Handle(UpdateServiceRequestCommand request,CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (currentUser is null ||
            !Guid.TryParse(currentUser.Id, out var clientId))
        {
            return false;
        }

        var serviceRequest =
            await serviceRequestRepository.GetByIdForClientAsync(
                request.RequestId,
                clientId,
                cancellationToken);

        if (serviceRequest is null ||
            serviceRequest.Status != ServiceRequestStatus.Open)
        {
            return false;
        }

        var categoryIsActive =
            await serviceRequestRepository.CategoryIsActiveAsync(
                request.CategoryId,
                cancellationToken);

        if (!categoryIsActive)
        {
            return false;
        }

        serviceRequest.CategoryId = request.CategoryId;
        serviceRequest.Title = request.Title?.Trim();
        serviceRequest.Description = request.Description.Trim();
        serviceRequest.IsUrgent = request.IsUrgent;
        serviceRequest.ScheduledFor = request.ScheduledFor;

        serviceRequest.AddressLine1 = request.AddressLine1.Trim();
        serviceRequest.AddressLine2 = request.AddressLine2?.Trim();
        serviceRequest.Neighborhood = request.Neighborhood?.Trim();
        serviceRequest.City = request.City.Trim();
        serviceRequest.State = request.State?.Trim();
        serviceRequest.PostalCode = request.PostalCode?.Trim();
        serviceRequest.CountryCode =
            request.CountryCode.Trim().ToUpperInvariant();
        serviceRequest.LocationReference =
            request.LocationReference?.Trim();

        serviceRequest.Latitude = request.Latitude;
        serviceRequest.Longitude = request.Longitude;
        serviceRequest.Location = new Point(
            (double)request.Longitude,
            (double)request.Latitude)
        {
            SRID = 4326
        };

        serviceRequest.UpdatedAt = DateTimeOffset.UtcNow;

        await serviceRequestRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}