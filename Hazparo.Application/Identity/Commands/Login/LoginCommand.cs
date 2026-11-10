using System.Net;
using Hazparo.Application.Identity.Dtos;
using MediatR;

public sealed class LoginCommand : IRequest<LoginResultDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public IPAddress? IpAddress { get; set; }
}