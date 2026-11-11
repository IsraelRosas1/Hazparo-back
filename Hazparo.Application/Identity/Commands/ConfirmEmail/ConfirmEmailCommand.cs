using MediatR;
using Microsoft.AspNetCore.Identity;
namespace Hazparo.Application.Identity.Commands.ConfirmEmail;
public sealed class ConfirmEmailCommand : IRequest<IdentityResult>
{
    public Guid UserId { get; set; }
    public string EncodedToken { get; set; } = string.Empty;
}