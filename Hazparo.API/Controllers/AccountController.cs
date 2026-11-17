using Hazparo.Application.Account.Commands.UpdateAccount;
using Hazparo.Application.Account.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hazparo.API.Controllers;

[ApiController]
[Route("api/account")]
[Authorize]
public sealed class AccountController(IMediator mediator): ControllerBase
{
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateAccountDto dto)
    {
        var result = await mediator.Send(
            new UpdateAccountCommand
            {
                DisplayName = dto.DisplayName
            });

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }
}