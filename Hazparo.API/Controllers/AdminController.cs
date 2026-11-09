using Hazparo.Application.Admin.Commands.AssignUserRole;
using Hazparo.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Hazparo.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles =UserRoles.Admin)]
public class AdminController(IMediator mediator) : ControllerBase
{

    [HttpPost("userRole")]
    public async Task<IActionResult> AssignUserRole(AssignUserRoleCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }
}
