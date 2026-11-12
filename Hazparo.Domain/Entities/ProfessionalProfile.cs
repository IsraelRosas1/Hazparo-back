namespace Hazparo.Domain.Entities;

public class ProfessionalProfile
{
    public Guid UserId { get; set; }
    public string? BusinessName { get; set; }
    public string? Bio { get; set; }
    public short YearsExperience { get; set; }
    public decimal? HourlyRateMxn { get; set; }
    public bool IsAvailable { get; set; }
    public bool AcceptsEmergency { get; set; }
    public short ServiceRadiusKm { get; set; } = 10;
    public VerificationStatus Verification { get; set; } = VerificationStatus.NotSubmitted;
    public DateTimeOffset? VerifiedAt { get; set; }
    public string? VerificationNotes { get; set; }
    public string? StripeCustomerId { get; set; }
    public bool SubscriptionActive { get; set; }
    public DateTimeOffset? SubscriptionExpiresAt { get; set; }
    public decimal RatingAverage { get; set; }
    public int RatingCount { get; set; }
    public int CompletedJobsCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public ProfessionalLocation? Location { get; set; }
    public ICollection<ProfessionalService> Services { get; set; } = new List<ProfessionalService>();
    public ICollection<ServiceRequest> AssignedRequests { get; set; } = new List<ServiceRequest>();
    public ICollection<ServiceQuote> Quotes { get; set; } = new List<ServiceQuote>();
    public ICollection<ProfessionalVerificationDocument> VerificationDocuments { get; set; } = new List<ProfessionalVerificationDocument>();
}
