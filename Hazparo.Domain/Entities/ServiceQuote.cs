namespace Hazparo.Domain.Entities;

public class ServiceQuote
{
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    public Guid ProfessionalId { get; set; }
    public decimal AmountMxn { get; set; }
    public string? Message { get; set; }
    public int? EstimatedArrivalMinutes { get; set; }
    public QuoteStatus Status { get; set; } = QuoteStatus.Sent;
    public DateTimeOffset? ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ServiceRequest Request { get; set; } = null!;
    public ProfessionalProfile Professional { get; set; } = null!;
}
