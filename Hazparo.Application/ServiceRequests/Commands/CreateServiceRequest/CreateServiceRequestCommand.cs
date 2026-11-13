using Hazparo.Application.ServiceRequests.Dtos;
using MediatR;

namespace Hazparo.Application.ServiceRequests.Commands.CreateServiceRequest;

public sealed class CreateServiceRequestCommand: IRequest<Guid?>
{
    public Guid CategoryId { get; set; }
    public string? Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsUrgent { get; set; }
    public DateTimeOffset? ScheduledFor { get; set; }

    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string? Neighborhood { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string CountryCode { get; set; } = "MX";
    public string? LocationReference { get; set; }

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public List<UploadedServiceRequestPhoto> Photos { get; set; } = [];
}