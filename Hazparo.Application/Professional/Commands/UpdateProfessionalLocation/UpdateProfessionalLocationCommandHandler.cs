using AutoMapper;
using Hazparo.Application.User;
using Hazparo.Domain.Entities;
using Hazparo.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;

public sealed class UpdateProfessionalLocationCommandHandler(
    IUserContext userContext,IProfessionalProfileRepository professionalProfileRepository,
    IMapper mapper)
    : IRequestHandler<UpdateProfessionalLocationCommand,IdentityResult>
{
    public async Task<IdentityResult> Handle(UpdateProfessionalLocationCommand request,CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (currentUser is null ||
            !Guid.TryParse(
                currentUser.Id,
                out var professionalId))
        {
            return Failed(
                "Unauthorized",
                "The authenticated user is invalid.");
        }

        var profile =
            await professionalProfileRepository.GetByUserIdAsync(
                professionalId,
                cancellationToken);

        if (profile is null)
        {
            return Failed(
                "ProfessionalProfileNotFound",
                "The professional profile was not found.");
        }

        var location =
            profile.Location ?? new ProfessionalLocation();

        mapper.Map(request, location);

        location.ProfessionalId = professionalId;
        location.AddressLine1 = request.AddressLine1.Trim();
        location.AddressLine2 =
            request.AddressLine2?.Trim();
        location.Neighborhood =
            request.Neighborhood?.Trim();
        location.City = request.City.Trim();
        location.State = request.State?.Trim();
        location.PostalCode =
            request.PostalCode?.Trim();
        location.CountryCode =
            request.CountryCode.Trim().ToUpperInvariant();

        location.Location = new Point(
            (double)request.Longitude,
            (double)request.Latitude)
        {
            SRID = 4326
        };

        location.UpdatedAt =
            DateTimeOffset.UtcNow;

        profile.Location = location;
        profile.UpdatedAt =
            DateTimeOffset.UtcNow;

        await professionalProfileRepository.SaveChangesAsync(
            cancellationToken);

        return IdentityResult.Success;
    }

    private static IdentityResult Failed(
        string code,
        string description)
    {
        return IdentityResult.Failed(
            new IdentityError
            {
                Code = code,
                Description = description
            });
    }
}