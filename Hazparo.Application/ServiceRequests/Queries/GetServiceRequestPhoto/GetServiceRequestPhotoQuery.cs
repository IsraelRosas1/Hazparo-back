using MediatR;

namespace Hazparo.Application.ServiceRequests.Queries.GetServiceRequestPhoto;

public sealed class GetServiceRequestPhotoQuery
    : IRequest<ServiceRequestPhotoFileDto?>
{
    public Guid RequestId { get; set; }
    public Guid PhotoId { get; set; }
}