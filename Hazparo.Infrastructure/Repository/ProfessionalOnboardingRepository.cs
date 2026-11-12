using Hazparo.Domain.Entities;
using Hazparo.Domain.Repositories;
using Hazparo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hazparo.Infrastructure.Persistence.Repositories;

public sealed class ProfessionalOnboardingRepository(
    HazparoDbContext dbContext)
    : IProfessionalOnboardingRepository
{
    public async Task SaveAsync(ProfessionalProfile profile, ProfessionalLocation location, IReadOnlyCollection<ProfessionalService> services,
        CancellationToken cancellationToken)
    {
        var existingProfile = await dbContext.ProfessionalProfiles.SingleOrDefaultAsync(
                    item => item.UserId == profile.UserId,
                    cancellationToken);

        if (existingProfile is null)
        {
            await dbContext.ProfessionalProfiles.AddAsync(
                profile,
                cancellationToken);
        }
        else
        {
            existingProfile.BusinessName = profile.BusinessName;
            existingProfile.Bio = profile.Bio;
            existingProfile.YearsExperience = profile.YearsExperience;
            existingProfile.HourlyRateMxn = profile.HourlyRateMxn;
            existingProfile.IsAvailable = profile.IsAvailable;
            existingProfile.AcceptsEmergency = profile.AcceptsEmergency;
            existingProfile.ServiceRadiusKm = profile.ServiceRadiusKm;
            existingProfile.UpdatedAt = profile.UpdatedAt;
        }

        var existingLocation =
            await dbContext.ProfessionalLocations
                .SingleOrDefaultAsync(
                    item => item.ProfessionalId == location.ProfessionalId,
                    cancellationToken);

        if (existingLocation is null)
        {
            await dbContext.ProfessionalLocations.AddAsync(
                location,
                cancellationToken);
        }
        else
        {
            existingLocation.Latitude = location.Latitude;
            existingLocation.Longitude = location.Longitude;
            existingLocation.Location = location.Location;
            existingLocation.City = location.City;
            existingLocation.UpdatedAt = location.UpdatedAt;
        }

        var existingServices =
            await dbContext.ProfessionalServices
                .Where(service =>
                    service.ProfessionalId == profile.UserId)
                .ToListAsync(cancellationToken);

        dbContext.ProfessionalServices.RemoveRange(
            existingServices);

        await dbContext.ProfessionalServices.AddRangeAsync(
            services,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}