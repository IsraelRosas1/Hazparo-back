using Hazparo.Application.Professional.Commands.BecomeProfessional;
using Hazparo.Application.Professional.Commands.UploadVerificationDocuments;
using Hazparo.Application.Professional.Dtos;
using Hazparo.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hazparo.API.Controllers;

[ApiController]
[Route("api/professionals")]
public class ProfessionalsController(IMediator mediator) : ControllerBase
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
                BusinessName = dto.BusinessName,
                Bio = dto.Bio,
                YearsExperience = dto.YearsExperience,
                HourlyRateMxn = dto.HourlyRateMxn,
                ServiceRadiusKm = dto.ServiceRadiusKm,
                AcceptsEmergency = dto.AcceptsEmergency,
                CategoryIds = dto.CategoryIds,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                City = dto.City
            });

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }
}