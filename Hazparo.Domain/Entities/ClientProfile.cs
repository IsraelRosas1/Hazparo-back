namespace Hazparo.Domain.Entities;

public class ClientProfile
{
    public Guid UserId { get; set; }
    public string? DefaultCity { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public AppUser User { get; set; } = null!;
    public ICollection<ClientAddress> Addresses { get; set; } = new List<ClientAddress>();
    public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
}
