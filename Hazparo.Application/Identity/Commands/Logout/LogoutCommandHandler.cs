using Hazparo.Domain.Interfaces;
using MediatR;

namespace Hazparo.Application.Identity.Commands.Logout;

public sealed class LogoutCommandHandler(
    IRefreshTokenService refreshTokenService)
    : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request,CancellationToken cancellationToken)
    {
        await refreshTokenService.RevokeAllAsync(request.UserId, cancellationToken);
    }
}