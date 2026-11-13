using Hazparo.Application.User;
using Hazparo.Domain.Constants;
using Hazparo.Domain.Entities;
using Hazparo.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;

namespace Hazparo.Application.Professional.Commands.BecomeProfessional;

public sealed class BecomeProfessionalCommandHandler(
    IUserContext userContext,
    UserManager<ApplicationUser> userManager,
    IServiceCategoryRepository serviceCategoryRepository,
    IProfessionalOnboardingRepository onboardingRepository)
    : IRequestHandler<
        BecomeProfessionalCommand,
        IdentityResult>
{
    public async Task<IdentityResult> Handle(
        BecomeProfessionalCommand request,
        CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (currentUser is null ||
            !Guid.TryParse(currentUser.Id, out var userId))
        {
            return Failed(
                "Unauthorized",
                "The authenticated user could not be identified.");
        }

        var user = await userManager.FindByIdAsync(
            userId.ToString());

        if (user is null)
        {
            return Failed(
                "UserNotFound",
                "The user was not found.");
        }

        var categoryIds = request.CategoryIds
            .Distinct()
            .ToList();

        var activeCategoryIds =
            await serviceCategoryRepository
                .GetActiveCategoryIdsAsync(
                    categoryIds,
                    cancellationToken);

        if (activeCategoryIds.Count != categoryIds.Count)
        {
            return Failed(
                "InvalidCategories",
                "One or more selected oficios are invalid or inactive.");
        }

        var roleResult = await AddProfessionalRoleIfNeeded(
            user);

        if (!roleResult.Succeeded)
        {
            return roleResult;
        }

        var now = DateTimeOffset.UtcNow;

        var profile = new ProfessionalProfile
        {
            UserId = userId,
            BusinessName = request.BusinessName?.Trim(),
            Bio = request.Bio?.Trim(),
            YearsExperience = request.YearsExperience,
            HourlyRateMxn = request.HourlyRateMxn,
            ServiceRadiusKm = request.ServiceRadiusKm,
            AcceptsEmergency = request.AcceptsEmergency,

            // Keep true for development testing.
            // In production, set false until documents are verified.
            IsAvailable = true,

            CreatedAt = now,
            UpdatedAt = now
        };

        var point = new Point(
            (double)request.Longitude,
            (double)request.Latitude)
        {
            SRID = 4326
        };

        var professionalLocation = new ProfessionalLocation
        {
            ProfessionalId = userId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Location = point,
            City = request.City?.Trim(),
            UpdatedAt = now
        };

        var professionalServices = categoryIds
            .Select((categoryId, index) =>
                new ProfessionalService
                {
                    ProfessionalId = userId,
                    CategoryId = categoryId,
                    IsPrimary = index == 0,
                    IsActive = true,
                    CreatedAt = now
                })
            .ToList();

        await onboardingRepository.SaveAsync(
            profile,
            professionalLocation,
            professionalServices,
            cancellationToken);

        return IdentityResult.Success;
    }

    private async Task<IdentityResult>
        AddProfessionalRoleIfNeeded(
            ApplicationUser user)
    {
        var alreadyProfessional =
            await userManager.IsInRoleAsync(
                user,
                UserRoles.Professional);

        if (alreadyProfessional)
        {
            return IdentityResult.Success;
        }

        return await userManager.AddToRoleAsync(
            user,
            UserRoles.Professional);
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