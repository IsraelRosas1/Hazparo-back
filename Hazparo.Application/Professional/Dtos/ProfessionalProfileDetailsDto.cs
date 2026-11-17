namespace Hazparo.Application.Professional.Dtos;

public sealed class ProfessionalProfileDetailsDto
{
    public Guid UserId { get; set; }

    public string DisplayName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    public string? BusinessName { get; set; }
    public string? Bio { get; set; }

    public decimal? HourlyRateMxn { get; set; }
    public short ServiceRadiusKm { get; set; }
    public bool IsAvailable { get; set; }

    public string Verification { get; set; } = string.Empty;
    public bool IsVerificationComplete { get; set; }

    public string? City { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public List<Guid> CategoryIds { get; set; } = [];
}