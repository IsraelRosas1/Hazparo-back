namespace Hazparo.Application.ServiceRequests.Commands
    .CreateServiceRequest;

public  class UploadedServiceRequestPhoto
{
    public Stream Content { get; set; } = null!;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Length { get; set; }
}