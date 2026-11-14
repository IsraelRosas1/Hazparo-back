using Hazparo.Application.ServiceRequests.Dtos;
using MediatR;

namespace Hazparo.Application.ServiceRequests.Queries.GetServiceRequestById;

public sealed class GetServiceRequestByIdQuery: IRequest<ServiceRequestDetailsDto?>
{
   public Guid RequestId { get; set; }
}