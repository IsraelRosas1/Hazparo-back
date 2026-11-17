namespace Hazparo.Application.Professional.Dtos;

public sealed class ProfessionalLocationDto
{
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string? Neighborhood { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string CountryCode { get; set; } = "MX";

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}