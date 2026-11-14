using Hazparo.Application.ServiceRequests.Commands.CreateServiceRequest;
using Hazparo.Application.ServiceRequests.Dtos;
using Hazparo.Application.ServiceRequests.Queries.GetMyServiceRequests;
using Hazparo.Application.ServiceRequests.Queries.GetServiceRequestPhoto;
using Hazparo.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hazparo.API.Controllers;

[ApiController]
[Route("api/service-requests")]
[Authorize(Roles = UserRoles.Client)]
public sealed class ServiceRequestsController(IMediator mediator): ControllerBase
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

            var requestId = await mediator.Send(
                new CreateServiceRequestCommand
                {
                    CategoryId = dto.CategoryId,
                    Title = dto.Title,
                    Description = dto.Description,
                    IsUrgent = dto.IsUrgent,
                    ScheduledFor = dto.ScheduledFor,
                    AddressLine1 = dto.AddressLine1,
                    AddressLine2 = dto.AddressLine2,
                    Neighborhood = dto.Neighborhood,
                    City = dto.City,
                    State = dto.State,
                    PostalCode = dto.PostalCode,
                    CountryCode = dto.CountryCode,
                    LocationReference = dto.LocationReference,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    Photos = uploadedPhotos
                });

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
}