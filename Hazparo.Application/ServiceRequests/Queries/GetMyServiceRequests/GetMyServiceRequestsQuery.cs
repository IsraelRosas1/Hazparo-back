using Hazparo.Application.ServiceRequests.Dtos;
using MediatR;

namespace Hazparo.Application.ServiceRequests.Queries
    .GetMyServiceRequests;

public class GetMyServiceRequestsQuery: IRequest<List<ServiceRequestSummaryDto>>
{
}