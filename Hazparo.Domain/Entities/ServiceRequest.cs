using NetTopologySuite.Geometries;
using Point = NetTopologySuite.Geometries.Point;

namespace Hazparo.Domain.Entities;

public class ServiceRequest
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public Guid CategoryId { get; set; }
    public string? Title { get; set; }
    public string Description { get; set; } = null!;
    public bool IsUrgent { get; set; }
    public DateTimeOffset? ScheduledFor { get; set; }
    public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Open;
    public Guid? AssignedProfessionalId { get; set; }
    public decimal? AgreedPriceMxn { get; set; }

    // Address snapshot retained with the request.
    public string AddressLine1 { get; set; } = null!;
    public string? AddressLine2 { get; set; }
    public string? Neighborhood { get; set; }
    public string City { get; set; } = null!;
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string CountryCode { get; set; } = "MX";
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? LocationReference { get; set; }
    public Point? Location { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? QuotedAt { get; set; }
    public DateTimeOffset? AssignedAt { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }
    public DateTimeOffset? EnRouteAt { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }
    public Guid? CancelledByUserId { get; set; }
    public string? CancellationReason { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    

    // The SQL schema stores the generated PostGIS geography value in `location`.
    // Its Npgsql/NetTopologySuite mapping will be added in the DbContext step.

    public ClientProfile Client { get; set; } = null!;
    public ServiceCategory Category { get; set; } = null!;
    public ProfessionalProfile? AssignedProfessional { get; set; }
    public ApplicationUser? CancelledByUser { get; set; }
    public ICollection<ServiceRequestPhoto> Photos { get; set; } = new List<ServiceRequestPhoto>();
    public ICollection<ServiceQuote> Quotes { get; set; } = new List<ServiceQuote>();
    public ICollection<ServiceRequestStatusHistory> StatusHistory { get; set; } = new List<ServiceRequestStatusHistory>();
}
