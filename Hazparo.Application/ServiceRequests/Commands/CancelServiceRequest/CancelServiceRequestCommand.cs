using MediatR;

namespace Hazparo.Application.ServiceRequests.Commands.CancelServiceRequest;

public sealed class CancelServiceRequestCommand : IRequest<bool>
{
    public Guid RequestId { get; set; }
    public string Reason { get; set; } = string.Empty;
}