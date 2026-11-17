using Hazparo.Application.User;
using Hazparo.Domain.Entities;
using Hazparo.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Hazparo.Application.Professional.Commands
    .UpdateProfessionalProfile;

public sealed class UpdateProfessionalProfileCommandHandler(
    IUserContext userContext,
    UserManager<ApplicationUser> userManager,
    IProfessionalProfileRepository professionalProfileRepository)
    : IRequestHandler<
        UpdateProfessionalProfileCommand,
        IdentityResult>
{
    public async Task<IdentityResult> Handle(UpdateProfessionalProfileCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (currentUser is null ||
            !Guid.TryParse(currentUser.Id, out var userId))
        {
            return Failed(
                "Unauthorized",
                "The authenticated user is invalid.");
        }

        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Failed(
                "UserNotFound",
                "The user was not found.");
        }

        var profile = await professionalProfileRepository.GetByUserIdAsync(userId,cancellationToken);

        if (profile is null)
        {
            return Failed("ProfessionalProfileNotFound", "The professional profile was not found.");
        }

        user.DisplayName = request.DisplayName.Trim();

        profile.BusinessName =
            string.IsNullOrWhiteSpace(request.BusinessName)
                ? null
                : request.BusinessName.Trim();
        profile.Bio =
            string.IsNullOrWhiteSpace(request.Bio)
                ? null
                : request.Bio.Trim();

        profile.HourlyRateMxn = request.HourlyRateMxn;
        profile.ServiceRadiusKm = request.ServiceRadiusKm;
        profile.IsAvailable = request.IsAvailable;
        profile.UpdatedAt = DateTimeOffset.UtcNow;

        var userResult =await userManager.UpdateAsync(user);

        if (!userResult.Succeeded)
        {
            return userResult;
        }

        await professionalProfileRepository.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    private static IdentityResult Failed(string code, string description)
    {
        return IdentityResult.Failed(
            new IdentityError
            {
                Code = code,
                Description = description
            });
    }
}