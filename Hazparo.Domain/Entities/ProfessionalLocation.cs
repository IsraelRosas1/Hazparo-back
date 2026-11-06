using NetTopologySuite.Geometries;
using Point = NetTopologySuite.Geometries.Point;

namespace Hazparo.Domain.Entities;

public class ProfessionalLocation
{
    public Guid ProfessionalId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public Point? Location { get; set; }
    public string? City { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    // The SQL schema stores the generated PostGIS geography value in `location`.
    // Its Npgsql/NetTopologySuite mapping will be added in the DbContext step.

    public ProfessionalProfile Professional { get; set; } = null!;
}
