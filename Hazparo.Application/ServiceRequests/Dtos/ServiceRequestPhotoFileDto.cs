namespace Hazparo.Application.ServiceRequests.Queries.GetServiceRequestPhoto;

public sealed class ServiceRequestPhotoFileDto
{
    public Stream Content { get; set; } = null!;
    public string ContentType { get; set; } = "application/octet-stream";
}