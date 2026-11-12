using Hazparo.Domain.Entities;

namespace Hazparo.Domain.Repositories;

public interface IProfessionalDocumentRepository
{
    Task<bool> ProfessionalExistsAsync( Guid professionalId, CancellationToken cancellationToken);
    Task<bool> DocumentsAlreadyExistAsync(Guid professionalId, CancellationToken cancellationToken);
    Task SaveAsync(IReadOnlyCollection<ProfessionalVerificationDocument> documents, CancellationToken cancellationToken);
}