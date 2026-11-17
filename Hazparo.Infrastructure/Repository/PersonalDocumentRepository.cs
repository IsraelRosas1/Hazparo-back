using Hazparo.Domain.Entities;
using Hazparo.Domain.Repositories;
using Hazparo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hazparo.Infrastructure.Persistence.Repositories;

public sealed class ProfessionalDocumentRepository(
    HazparoDbContext dbContext)
    : IProfessionalDocumentRepository
{
    public Task<bool> ProfessionalExistsAsync(Guid professionalId,CancellationToken cancellationToken)
    {
        return dbContext.ProfessionalProfiles
            .AnyAsync(
                profile => profile.UserId == professionalId,
                cancellationToken);
    }

    public Task<VerificationStatus?> GetVerificationStatusAsync(Guid professionalId, CancellationToken cancellationToken)
    {
        return dbContext.ProfessionalProfiles
            .Where(profile => profile.UserId == professionalId)
            .Select(profile => (VerificationStatus?)profile.Verification)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProfessionalVerificationDocument>>GetByProfessionalIdAsync(
            Guid professionalId,
            CancellationToken cancellationToken)
    {
        return await dbContext
            .ProfessionalVerificationDocuments
            .AsNoTracking()
            .Where(document =>
                document.ProfessionalId == professionalId)
            .ToListAsync(cancellationToken);
    }

    public async Task ReplaceAsync(Guid professionalId,
        IReadOnlyCollection<ProfessionalVerificationDocument> documents,
        CancellationToken cancellationToken)
    {
        var existingDocuments =
            await dbContext
                .ProfessionalVerificationDocuments
                .Where(document =>
                    document.ProfessionalId == professionalId)
                .ToListAsync(cancellationToken);

        dbContext.ProfessionalVerificationDocuments.RemoveRange(
            existingDocuments);

        await dbContext
            .ProfessionalVerificationDocuments
            .AddRangeAsync(
                documents,
                cancellationToken);

        var profile =
            await dbContext.ProfessionalProfiles
                .SingleAsync(
                    professional =>
                        professional.UserId == professionalId,
                    cancellationToken);

        profile.Verification =VerificationStatus.Pending;

        profile.UpdatedAt =DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}