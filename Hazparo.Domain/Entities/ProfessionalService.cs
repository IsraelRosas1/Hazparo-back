namespace Hazparo.Domain.Entities;

public class ProfessionalService
{
    public Guid ProfessionalId { get; set; }
    public Guid CategoryId { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }

    public ProfessionalProfile Professional { get; set; } = null!;
    public ServiceCategory Category { get; set; } = null!;
}
