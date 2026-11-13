using Hazparo.Application.Identity.Dtos;
using Hazparo.Domain.Entities;
using Hazparo.Domain.Exceptions;
using Hazparo.Domain.Interfaces;
using Hazparo.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Hazparo.Application.Identity.Commands.Login;

public class LoginCommandHandler(
    UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,
    ILogger<LoginCommandHandler> logger,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService)
    : IRequestHandler<LoginCommand, LoginResultDto>
{
    public async Task<LoginResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return new LoginResultDto
            {
                Succeeded = false,
                Error = "Email o contraseña inválidos."
            };        
        }
        var signInResult= await signInManager.CheckPasswordSignInAsync(user,request.Password,lockoutOnFailure:true);

        if(signInResult.IsLockedOut)
        {
            return new LoginResultDto
            {
                IsLockedOut = true,
                Error = "La cuenta esta bloqueada"
            };
        }
        
        // if (signInResult.IsNotAllowed)
        // {
        //     return new LoginResultDto
        //     {
        //         RequiresConfirmedEmail = true,
        //         Error = "Confirm your email before signing in."
        //     };
        // }

        if(!signInResult.Succeeded )
        {
            return new LoginResultDto
            {
                Error = "Email o contrasena invalidos"
            };
        }

        var accessToken = await accessTokenService.CreateAsync(user);
        var refreshToken = await refreshTokenService.IssueAsync(user.Id, request.UserAgent,request.IpAddress,cancellationToken );
        return new LoginResultDto
        {
            Succeeded = true,
            Tokens = new AuthenticationTokensDto
            {
                AccessToken = accessToken.AccessToken,
                AccessTokenExpiresAt = accessToken.ExpiresAt,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAt = refreshToken.ExpiresAt
            }
        };
    } 
}