namespace Hazparo.Application.ServiceRequests.Dtos;

public sealed class ServiceRequestPhotoDto
{
    public Guid Id { get; set; }
    public string? ContentType { get; set; }
    public short SortOrder { get; set; }
}