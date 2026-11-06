namespace Hazparo.Domain.Entities;

public class ServiceRequestPhoto
{
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    public string ObjectKey { get; set; } = null!;
    public string? ContentType { get; set; }
    public short SortOrder { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public ServiceRequest Request { get; set; } = null!;
}
