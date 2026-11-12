using MediatR;

namespace Hazparo.Application.Identity.Commands.Logout;

public class LogoutCommand : IRequest
{
    public Guid UserId { get; set; }
}