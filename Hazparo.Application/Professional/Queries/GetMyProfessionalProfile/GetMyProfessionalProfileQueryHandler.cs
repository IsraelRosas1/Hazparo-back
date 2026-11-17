using Hazparo.Application.Professional.Dtos;
using Hazparo.Application.User;
using Hazparo.Domain.Entities;
using Hazparo.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Hazparo.Application.Professional.Queries
    .GetMyProfessionalProfile;

public sealed class GetMyProfessionalProfileQueryHandler(
    IUserContext userContext,
    UserManager<ApplicationUser> userManager,
    IProfessionalProfileRepository professionalProfileRepository)
    : IRequestHandler<
        GetMyProfessionalProfileQuery,
        ProfessionalProfileDetailsDto?>
{
    public async Task<ProfessionalProfileDetailsDto?> Handle(
        GetMyProfessionalProfileQuery request,
        CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (currentUser is null ||
            !Guid.TryParse(currentUser.Id, out var userId))
        {
            return null;
        }

        var user = await userManager.FindByIdAsync(
            userId.ToString());

        if (user is null)
        {
            return null;
        }

        var profile =
            await professionalProfileRepository.GetByUserIdAsync(
                userId,
                cancellationToken);

        if (profile is null)
        {
            return null;
        }

        return new ProfessionalProfileDetailsDto
        {
            UserId = user.Id,
            DisplayName = user.DisplayName,
            PhoneNumber = user.PhoneNumber,

            BusinessName = profile.BusinessName,
            Bio = profile.Bio,
            HourlyRateMxn = profile.HourlyRateMxn,
            ServiceRadiusKm = profile.ServiceRadiusKm,
            IsAvailable = profile.IsAvailable,

            Verification = profile.Verification.ToString(),
            IsVerificationComplete =
                profile.Verification ==
                VerificationStatus.Verified,

            City = profile.Location?.City,
            Latitude = profile.Location?.Latitude,
            Longitude = profile.Location?.Longitude,

            CategoryIds = profile.Services
                .Where(service => service.IsActive)
                .Select(service => service.CategoryId)
                .ToList()
        };
    }
}