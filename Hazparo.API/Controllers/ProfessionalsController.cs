using AutoMapper;
using Hazparo.Application.Professional.Commands.BecomeProfessional;
using Hazparo.Application.Professional.Commands.UpdateProfessionalProfile;
using Hazparo.Application.Professional.Commands.UploadVerificationDocuments;
using Hazparo.Application.Professional.Dtos;
using Hazparo.Application.Professional.Queries.GetMyProfessionalProfile;
using Hazparo.Application.Professional.Queries.GetMyVerificationStatus;
using Hazparo.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hazparo.API.Controllers;

[ApiController]
[Route("api/professionals")]
public class ProfessionalsController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost("verification-documents")]
    [Authorize(Roles = UserRoles.Professional)]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(15_000_000)]
    public async Task<IActionResult> UploadVerificationDocuments(
        [FromForm] IFormFile? facePhoto,
        [FromForm] IFormFile? ineFrontPhoto,
        [FromForm] IFormFile? ineBackPhoto)
    {
        if (facePhoto is null ||
            ineFrontPhoto is null ||
            ineBackPhoto is null)
        {
            return BadRequest(
                "facePhoto, ineFrontPhoto, and ineBackPhoto are required.");
        }

        await using var faceStream =
            facePhoto.OpenReadStream();

        await using var ineFrontStream =
            ineFrontPhoto.OpenReadStream();

        await using var ineBackStream =
            ineBackPhoto.OpenReadStream();

        var result = await mediator.Send(
            new UploadVerificationDocumentsCommand
            {
                FacePhoto = new UploadedProfessionalFile
                {
                    Content = faceStream,
                    OriginalFileName = facePhoto.FileName,
                    ContentType = facePhoto.ContentType,
                    Length = facePhoto.Length
                },
                IneFrontPhoto = new UploadedProfessionalFile
                {
                    Content = ineFrontStream,
                    OriginalFileName = ineFrontPhoto.FileName,
                    ContentType = ineFrontPhoto.ContentType,
                    Length = ineFrontPhoto.Length
                },
                IneBackPhoto = new UploadedProfessionalFile
                {
                    Content = ineBackStream,
                    OriginalFileName = ineBackPhoto.FileName,
                    ContentType = ineBackPhoto.ContentType,
                    Length = ineBackPhoto.Length
                }
            });

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    [HttpPost("onboarding")]
    [Authorize]
    public async Task<IActionResult> BecomeProfessional(
        [FromBody] ProfessionalOnboardingDto dto)
    {
        var result = await mediator.Send(
            new BecomeProfessionalCommand
            {
                DisplayName = dto.DisplayName,
                PhoneNumber = dto.PhoneNumber,
                BusinessName = dto.BusinessName,
                Bio = dto.Bio,
                YearsExperience = dto.YearsExperience,
                HourlyRateMxn = dto.HourlyRateMxn,
                ServiceRadiusKm = dto.ServiceRadiusKm,
                AcceptsEmergency = dto.AcceptsEmergency,
                CategoryIds = dto.CategoryIds,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                Neighborhood = dto.Neighborhood,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                CountryCode = dto.CountryCode
            });

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }
    [HttpPut("me")]
    [Authorize(Roles = UserRoles.Professional)]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfessionalProfileDto dto)
    {
        var result = await mediator.Send(
            new UpdateProfessionalProfileCommand
            {
                DisplayName = dto.DisplayName,
                BusinessName = dto.BusinessName,
                Bio = dto.Bio,
                HourlyRateMxn = dto.HourlyRateMxn,
                ServiceRadiusKm = dto.ServiceRadiusKm,
                IsAvailable = dto.IsAvailable
            });

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    [HttpGet("me")]
    [Authorize(Roles = UserRoles.Professional)]
    public async Task<ActionResult<ProfessionalProfileDetailsDto>>GetMyProfile()
    {
        var result = await mediator.Send(new GetMyProfessionalProfileQuery());

        if (result is null)
        {
            return NotFound(
                "The professional profile was not found.");
        }

        return Ok(result);
    }

    [HttpGet("me/location")]
    [Authorize(Roles = UserRoles.Professional)]
    public async Task<ActionResult<ProfessionalLocationDto>>GetMyLocation()
    {
        var result = await mediator.Send(new GetMyProfessionalLocationQuery());

        if (result is null)
        {
            return NotFound(
                "The professional location was not found.");
        }

        return Ok(result);
    }

    [HttpPut("me/location")]
    [Authorize(Roles = UserRoles.Professional)]
    public async Task<IActionResult> UpdateMyLocation([FromBody] ProfessionalLocationDto dto)
    {
        var command =mapper.Map<UpdateProfessionalLocationCommand>(dto);

        var result = await mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    [HttpGet("me/verification-status")]
    [Authorize(Roles = UserRoles.Professional)]
    public async Task<
        ActionResult<ProfessionalVerificationStatusDto>>GetMyVerificationStatus()
    {
        var result = await mediator.Send(new GetMyVerificationStatusQuery());

        if (result is null)
        {
            return NotFound("The professional profile was not found.");
        }

        return Ok(result);
    }

}