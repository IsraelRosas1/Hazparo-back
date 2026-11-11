
using System.Net;
using Hazparo.Application.Identity.Dtos;
using MediatR;

namespace Hazparo.Application.Identity.Commands.RefreshCommand;
public sealed class RefreshCommand : IRequest<AuthenticationTokensDto?>
{
    public string RefreshToken { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public IPAddress? IpAddress { get; set; }
}