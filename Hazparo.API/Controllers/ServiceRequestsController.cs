using AutoMapper;
using Hazparo.Application.ServiceRequests.Commands.CancelServiceRequest;
using Hazparo.Application.ServiceRequests.Commands.CreateServiceRequest;
using Hazparo.Application.ServiceRequests.Commands.UpdateServiceRequest;
using Hazparo.Application.ServiceRequests.Dtos;
using Hazparo.Application.ServiceRequests.Queries.GetMyServiceRequests;
using Hazparo.Application.ServiceRequests.Queries.GetServiceRequestById;
using Hazparo.Application.ServiceRequests.Queries.GetServiceRequestPhoto;
using Hazparo.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hazparo.API.Controllers;

[ApiController]
[Route("api/service-requests")]
[Authorize(Roles = UserRoles.Client)]
public sealed class ServiceRequestsController(IMediator mediator, IMapper mapper): ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(45_000_000)]
    public async Task<IActionResult> Create(
        [FromForm] CreateServiceRequestDto dto,
        [FromForm] List<IFormFile>? photos)
    {
        if (photos is { Count: > 8 })
        {
            return BadRequest(
                "A service request can contain up to 8 photos.");
        }

        var streams = new List<Stream>();

        try
        {
            var uploadedPhotos = new List<
                UploadedServiceRequestPhoto>();

            foreach (var photo in photos ?? [])
            {
                var stream = photo.OpenReadStream();

                streams.Add(stream);

                uploadedPhotos.Add(
                    new UploadedServiceRequestPhoto
                    {
                        Content = stream,
                        OriginalFileName = photo.FileName,
                        ContentType = photo.ContentType,
                        Length = photo.Length
                    });
            }
            var command = mapper.Map<CreateServiceRequestCommand>(dto);
            command.Photos = uploadedPhotos;
            var requestId = await mediator.Send(command);


            if (requestId is null)
            {
                return BadRequest(
                    "Could not create the service request.");
            }

            return Created(
                $"/api/service-requests/{requestId}",
                new { id = requestId });
        }
        finally
        {
            foreach (var stream in streams)
            {
                await stream.DisposeAsync();
            }
        }
    }

    [HttpGet("mine")]
    public async Task<
        ActionResult<List<ServiceRequestSummaryDto>>>
        GetMine()
    {
        var result = await mediator.Send(
            new GetMyServiceRequestsQuery());

        return Ok(result);
    }

    [HttpGet("{requestId:guid}/photos/{photoId:guid}")]
    public async Task<IActionResult> GetPhoto([FromRoute] Guid requestId, [FromRoute] Guid photoId)
    {
        var result = await mediator.Send(
            new GetServiceRequestPhotoQuery
            {
                RequestId = requestId,
                PhotoId = photoId
            });

        if (result is null)
        {
            return NotFound();
        }

        return File(
            result.Content,
            result.ContentType);
    }
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ServiceRequestDetailsDto>> GetById([FromRoute] Guid id)
    {
        var result = await mediator.Send(
            new GetServiceRequestByIdQuery
            {
                RequestId = id
            });

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id,[FromBody] UpdateServiceRequestDto dto)
    {
        var command = mapper.Map<UpdateServiceRequestCommand>(dto);
        command.RequestId = id;
        var updated = await mediator.Send(command);


        if (!updated)
        {
            return BadRequest(
                "The job was not found, is not open, or the category is invalid.");
        }

        return NoContent();
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, [FromBody] CancelServiceRequestDto dto)
    {
        var cancelled = await mediator.Send(
            new CancelServiceRequestCommand
            {
                RequestId = id,
                Reason = dto.Reason
            });

        if (!cancelled)
        {
            return BadRequest(
                "The job was not found or can no longer be cancelled.");
        }

        return NoContent();
    }
}