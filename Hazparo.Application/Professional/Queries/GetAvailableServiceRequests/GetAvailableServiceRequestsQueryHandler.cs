using AutoMapper;
using Hazparo.Application.Professional.Dtos;
using Hazparo.Application.User;
using Hazparo.Domain.Repositories;
using MediatR;

namespace Hazparo.Application.Professional.Queries
    .GetAvailableServiceRequests;

public sealed class GetAvailableServiceRequestsQueryHandler(IUserContext userContext,IServiceRequestRepository serviceRequestRepository,
    IMapper mapper)
    : IRequestHandler<GetAvailableServiceRequestsQuery,List<AvailableProfessionalServiceRequestDto>>
{
    public async Task<List<AvailableProfessionalServiceRequestDto>> Handle(GetAvailableServiceRequestsQuery request,CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (currentUser is null ||
            !Guid.TryParse(
                currentUser.Id,
                out var professionalId))
        {
            return [];
        }

        var serviceRequests =
            await serviceRequestRepository
                .GetAvailableForProfessionalAsync(
                    professionalId,
                    cancellationToken);

        return mapper.Map<List<AvailableProfessionalServiceRequestDto>>(serviceRequests);
    }
}