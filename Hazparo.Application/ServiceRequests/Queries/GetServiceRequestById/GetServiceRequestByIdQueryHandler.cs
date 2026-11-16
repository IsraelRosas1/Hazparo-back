using AutoMapper;
using Hazparo.Application.ServiceRequests.Dtos;
using Hazparo.Application.User;
using Hazparo.Domain.Repositories;
using MediatR;

namespace Hazparo.Application.ServiceRequests.Queries.GetServiceRequestById;

public sealed class GetServiceRequestByIdQueryHandler(
    IUserContext userContext,IServiceRequestRepository serviceRequestRepository,
    IMapper mapper)
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

        return mapper.Map<ServiceRequestDetailsDto>(
            serviceRequest);

    }
}