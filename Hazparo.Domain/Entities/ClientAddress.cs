namespace Hazparo.Domain.Entities;

using NetTopologySuite.Geometries;
using Point = NetTopologySuite.Geometries.Point;
public class ClientAddress
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string Label { get; set; } = "Casa";
    public string? RecipientName { get; set; }
    public string? PhoneNumber { get; set; }
    public string AddressLine1 { get; set; } = null!;
    public string? AddressLine2 { get; set; }
    public string? Neighborhood { get; set; }
    public string City { get; set; } = null!;
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string CountryCode { get; set; } = "MX";
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsDefault { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    // The SQL schema stores a PostGIS geography value in `location`.
    // Its Npgsql/NetTopologySuite mapping will be added in the DbContext step.
    public Point? Location { get; set; }
    public ClientProfile Client { get; set; } = null!;
}
