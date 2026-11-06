namespace Hazparo.Domain.Entities;

public class ServiceRequestStatusHistory
{
    public long Id { get; set; }
    public Guid RequestId { get; set; }
    public ServiceRequestStatus? FromStatus { get; set; }
    public ServiceRequestStatus ToStatus { get; set; }
    public Guid? ChangedByUserId { get; set; }
    public string? Note { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public ServiceRequest Request { get; set; } = null!;
    public AppUser? ChangedByUser { get; set; }
}
