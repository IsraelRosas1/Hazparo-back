
using System.Security.Claims;
using Hazparo.Application.Identity.Commands.Login;
using Hazparo.Application.Identity.Commands.Logout;
using Hazparo.Application.Identity.Commands.RefreshCommand;
using Hazparo.Application.Identity.Commands.Register;
using Hazparo.Application.Identity.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Hazparo.API.Controllers;

[ApiController]
[Route("api/identity")]
public class IdentityController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] RegisterDto request)
    {
        var result = await mediator.Send(new RegisterCommand
        {
            Email = request.Email,
            Password = request.Password,
            DisplayName = request.DisplayName
        });

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Accepted();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("identity-login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto dto)
    {
        var result = await mediator.Send(new LoginCommand
        {
            Email = dto.Email,
            Password = dto.Password,
            UserAgent = Request.Headers.UserAgent.ToString(),
            IpAddress = HttpContext.Connection.RemoteIpAddress
        });

        if (!result.Succeeded || result.Tokens is null)
        {
            return Unauthorized(result.Error);
        }

        return Ok(result.Tokens);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting("identity-refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenDto dto)
    {
        var result = await mediator.Send(new RefreshCommand
        {
            RefreshToken = dto.RefreshToken,
            UserAgent = Request.Headers.UserAgent.ToString(),
            IpAddress = HttpContext.Connection.RemoteIpAddress
        });

        if (result is null)
        {
            return Unauthorized();
        }

        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        await mediator.Send(new LogoutCommand
        {
            UserId = userId
        });

        return NoContent();
    }
}