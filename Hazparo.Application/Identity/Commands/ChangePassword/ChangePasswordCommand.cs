using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Hazparo.Application.Identity.Commands.ChangePassword;

public sealed class ChangePasswordCommand : IRequest<IdentityResult>
{
    public Guid UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}