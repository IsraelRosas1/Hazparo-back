using Hazparo.Domain.Entities;
using Hazparo.Domain.Repositories;
using Hazparo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hazparo.Infrastructure.Persistence.Repositories;

public sealed class ProfessionalDocumentRepository(
    HazparoDbContext dbContext)
    : IProfessionalDocumentRepository
{
    public Task<bool> ProfessionalExistsAsync(
        Guid professionalId,
        CancellationToken cancellationToken)
    {
        return dbContext.ProfessionalProfiles
            .AnyAsync(
                profile => profile.UserId == professionalId,
                cancellationToken);
    }

    public Task<bool> DocumentsAlreadyExistAsync(
        Guid professionalId,
        CancellationToken cancellationToken)
    {
        return dbContext.ProfessionalVerificationDocuments
            .AnyAsync(
                document =>
                    document.ProfessionalId == professionalId,
                cancellationToken);
    }

    public async Task SaveAsync(
        IReadOnlyCollection<ProfessionalVerificationDocument> documents,
        CancellationToken cancellationToken)
    {
        await dbContext.ProfessionalVerificationDocuments.AddRangeAsync(
            documents,
            cancellationToken);

        var professionalId = documents
            .Select(document => document.ProfessionalId)
            .First();

        var profile =
            await dbContext.ProfessionalProfiles
                .SingleAsync(
                    item => item.UserId == professionalId,
                    cancellationToken);

        profile.Verification =
            VerificationStatus.Pending;

        profile.UpdatedAt =
            DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}