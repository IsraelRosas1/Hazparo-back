using Hazparo.Domain.Entities;

namespace Hazparo.Domain.Repositories;

public interface IProfessionalDocumentRepository
{
    Task<bool> ProfessionalExistsAsync(Guid professionalId,CancellationToken cancellationToken);

    Task<VerificationStatus?> GetVerificationStatusAsync(Guid professionalId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ProfessionalVerificationDocument>>GetByProfessionalIdAsync(Guid professionalId, CancellationToken cancellationToken);

    Task ReplaceAsync(Guid professionalId,IReadOnlyCollection<ProfessionalVerificationDocument> documents,
        CancellationToken cancellationToken);
}