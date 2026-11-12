using Hazparo.Application.Identity.Dtos;
using Hazparo.Domain.Entities;
using Hazparo.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Hazparo.Application.Identity.Commands.RefreshCommand;

public sealed class RefreshCommandHandler(
    UserManager<ApplicationUser> userManager,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService)
    : IRequestHandler<RefreshCommand, AuthenticationTokensDto?>
{
    public async Task<AuthenticationTokensDto?> Handle( RefreshCommand request, CancellationToken cancellationToken)
    {
        var rotatedToken =
            await refreshTokenService.RotateAsync(
                request.RefreshToken,
                request.UserAgent,
                request.IpAddress,
                cancellationToken);

        if (rotatedToken is null)
        {
            return null;
        }

        var user = await userManager.FindByIdAsync(
            rotatedToken.UserId.ToString());

        //user.Status != AccountStatus.Active ||!user.EmailConfirmed
        if (user is null )
        {
            await refreshTokenService.RevokeAllAsync(
                rotatedToken.UserId,
                cancellationToken);

            return null;
        }

        var accessToken =
            await accessTokenService.CreateAsync(user);

        return new AuthenticationTokensDto
        {
            AccessToken = accessToken.AccessToken,
            AccessTokenExpiresAt = accessToken.ExpiresAt,
            RefreshToken = rotatedToken.Token,
            RefreshTokenExpiresAt = rotatedToken.ExpiresAt
        };
    }
}