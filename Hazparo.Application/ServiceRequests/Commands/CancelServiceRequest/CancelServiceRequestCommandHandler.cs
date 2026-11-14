using Hazparo.Application.User;
using Hazparo.Domain.Entities;
using Hazparo.Domain.Repositories;
using MediatR;

namespace Hazparo.Application.ServiceRequests.Commands.CancelServiceRequest;

public sealed class CancelServiceRequestCommandHandler(
    IUserContext userContext,
    IServiceRequestRepository serviceRequestRepository)
    : IRequestHandler<CancelServiceRequestCommand, bool>
{
    public async Task<bool> Handle(
        CancelServiceRequestCommand request,
        CancellationToken cancellationToken)
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

        var now = DateTimeOffset.UtcNow;
        var previousStatus = serviceRequest.Status;

        serviceRequest.Status = ServiceRequestStatus.Cancelled;
        serviceRequest.CancelledAt = now;
        serviceRequest.CancelledByUserId = clientId;
        serviceRequest.CancellationReason = request.Reason.Trim();
        serviceRequest.UpdatedAt = now;

        serviceRequest.StatusHistory.Add(
            new ServiceRequestStatusHistory
            {
                RequestId = serviceRequest.Id,
                FromStatus = previousStatus,
                ToStatus = ServiceRequestStatus.Cancelled,
                ChangedByUserId = clientId,
                Note = request.Reason.Trim(),
                CreatedAt = now
            });

        await serviceRequestRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}