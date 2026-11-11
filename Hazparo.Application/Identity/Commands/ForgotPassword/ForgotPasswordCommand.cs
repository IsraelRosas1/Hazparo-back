using MediatR;

namespace Hazparo.Application.Identity.Commands.ForgotPassword;
public sealed class ForgotPasswordCommand : IRequest
{
    public string Email { get; set; } = string.Empty;
}