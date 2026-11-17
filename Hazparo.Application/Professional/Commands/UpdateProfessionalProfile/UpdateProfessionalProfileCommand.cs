using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Hazparo.Application.Professional.Commands.UpdateProfessionalProfile;

public sealed class UpdateProfessionalProfileCommand: IRequest<IdentityResult>
{
    public string DisplayName { get; set; } = string.Empty;
    public string? BusinessName { get; set; }
    public string? Bio { get; set; }

    public decimal? HourlyRateMxn { get; set; }
    public short ServiceRadiusKm { get; set; }
    public bool IsAvailable { get; set; }
}