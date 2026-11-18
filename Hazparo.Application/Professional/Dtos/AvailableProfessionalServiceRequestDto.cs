using Hazparo.Domain.Entities;

namespace Hazparo.Application.Professional.Dtos;

public sealed class AvailableProfessionalServiceRequestDto
{
    public Guid Id { get; set; }

    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public string? Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsUrgent { get; set; }
    public DateTimeOffset? ScheduledFor { get; set; }
    public ServiceRequestStatus Status { get; set; }

    public string AddressLine1 { get; set; } = string.Empty;
    public string? Neighborhood { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? LocationReference { get; set; }

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}