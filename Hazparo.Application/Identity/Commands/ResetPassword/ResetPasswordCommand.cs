using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Hazparo.Application.Identity.Commands.ResetPassword;

public sealed class ResetPasswordCommand : IRequest<IdentityResult>
{
    public string Email { get; set; } = string.Empty;
    public string EncodedToken { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}