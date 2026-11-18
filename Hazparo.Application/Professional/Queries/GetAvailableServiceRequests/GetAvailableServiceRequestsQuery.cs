using Hazparo.Application.Professional.Dtos;
using MediatR;

namespace Hazparo.Application.Professional.Queries
    .GetAvailableServiceRequests;

public sealed class GetAvailableServiceRequestsQuery: IRequest<List<AvailableProfessionalServiceRequestDto>>
{
}