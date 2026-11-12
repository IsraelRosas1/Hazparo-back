using Hazparo.Domain.Entities;

namespace Hazparo.Domain.Repositories;

public interface IProfessionalOnboardingRepository
{
    Task SaveAsync(ProfessionalProfile profile, ProfessionalLocation location, 
    IReadOnlyCollection<ProfessionalService> services, CancellationToken cancellationToken);
}