using Hazparo.Application.ServiceCategories.Dtos;
using Hazparo.Application.ServiceCategories.Queries.GetServiceCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hazparo.API.Controllers;

[ApiController]
[Route("api/service-categories")]
public sealed class ServiceCategoriesController(IMediator mediator): ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<ServiceCategoryGroupDto>>>GetServiceCategories()
    {
        var result = await mediator.Send(new GetServiceCategoriesQuery());

        return Ok(result);
    }
}