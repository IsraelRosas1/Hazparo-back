using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Hazparo.Application.Professional.Commands.BecomeProfessional;

public sealed class BecomeProfessionalCommand
    : IRequest<IdentityResult>
{
    public string? BusinessName { get; set; }
    public string? Bio { get; set; }
    public short YearsExperience { get; set; }
    public decimal? HourlyRateMxn { get; set; }
    public short ServiceRadiusKm { get; set; }
    public bool AcceptsEmergency { get; set; }
    public List<Guid> CategoryIds { get; set; } = [];
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? City { get; set; }
}