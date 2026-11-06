namespace Hazparo.Domain.Entities;

public class ServiceCategory
{
    public Guid Id { get; set; }
    public string GroupName { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? IconKey { get; set; }
    public string[] SearchAliases { get; set; } = Array.Empty<string>();
    public bool IsActive { get; set; } = true;
    public short SortOrder { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<ProfessionalService> ProfessionalServices { get; set; } = new List<ProfessionalService>();
    public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
}
