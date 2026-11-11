
using Hazparo.Application.Identity.Commands.Register;
using Hazparo.Application.Identity.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

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
}