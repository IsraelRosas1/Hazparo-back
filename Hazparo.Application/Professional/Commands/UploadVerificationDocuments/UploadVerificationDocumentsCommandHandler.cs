using Hazparo.Application.User;
using Hazparo.Domain.Entities;
using Hazparo.Domain.Interfaces;
using Hazparo.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Hazparo.Application.Professional.Commands
    .UploadVerificationDocuments;

public sealed class UploadVerificationDocumentsCommandHandler(
    IUserContext userContext,
    IPrivateFileStorage privateFileStorage,
    IProfessionalDocumentRepository documentRepository,
    ILogger<UploadVerificationDocumentsCommandHandler> logger)
    : IRequestHandler<
        UploadVerificationDocumentsCommand,
        IdentityResult>
{
    private const long MaxFileSize = 5 * 1024 * 1024;

    private static readonly HashSet<string>
        AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/jpg",
            "image/png",
            "image/heic",
            "image/heif",
            "image/webp",
            "image/avif"
        };

    public async Task<IdentityResult> Handle(
        UploadVerificationDocumentsCommand request,
        CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (currentUser is null ||
            !Guid.TryParse(
                currentUser.Id,
                out var professionalId))
        {
            return Failed(
                "Unauthorized",
                "The authenticated user could not be identified.");
        }

        var verificationStatus =
            await documentRepository.GetVerificationStatusAsync(
                professionalId,
                cancellationToken);

        if (verificationStatus is null)
        {
            return Failed(
                "ProfessionalProfileNotFound",
                "Complete professional onboarding first.");
        }

        if (verificationStatus ==
            VerificationStatus.Pending)
        {
            return Failed(
                "VerificationPending",
                "Your verification documents are already being reviewed.");
        }

        if (verificationStatus ==
            VerificationStatus.Verified)
        {
            return Failed(
                "AlreadyVerified",
                "Your account has already been verified.");
        }

        var files = new[]
        {
            (
                File: request.FacePhoto,
                Type: ProfessionalDocumentType.FacePhoto,
                Name: "face-photo"
            ),
            (
                File: request.IneFrontPhoto,
                Type: ProfessionalDocumentType.IneFront,
                Name: "ine-front"
            ),
            (
                File: request.IneBackPhoto,
                Type: ProfessionalDocumentType.IneBack,
                Name: "ine-back"
            )
        };

        foreach (var item in files)
        {
            if (item.File.Length <= 0 ||
                item.File.Length > MaxFileSize)
            {
                return Failed(
                    "InvalidFileSize",
                    "Each image must be between 1 byte and 5 MB.");
            }

            if (!AllowedContentTypes.Contains(
                    item.File.ContentType))
            {
                return Failed(
                    "InvalidFileType",
                    "The uploaded file type is not supported.");
            }
        }

        var oldDocuments =
            await documentRepository.GetByProfessionalIdAsync(
                professionalId,
                cancellationToken);

        var savedKeys = new List<string>();
        var newDocuments =
            new List<ProfessionalVerificationDocument>();

        var now = DateTimeOffset.UtcNow;

        try
        {
            foreach (var item in files)
            {
                var storageKey =
                    await privateFileStorage.SaveAsync(
                        item.File.Content,
                        professionalId,
                        item.Name,
                        cancellationToken);

                savedKeys.Add(storageKey);

                newDocuments.Add(
                    new ProfessionalVerificationDocument
                    {
                        Id = Guid.NewGuid(),
                        ProfessionalId = professionalId,
                        DocumentType = item.Type,
                        StorageKey = storageKey,
                        OriginalFileName =
                            Path.GetFileName(
                                item.File.OriginalFileName),
                        ContentType = item.File.ContentType,
                        SizeBytes = item.File.Length,
                        CreatedAt = now
                    });
            }

            await documentRepository.ReplaceAsync(
                professionalId,
                newDocuments,
                cancellationToken);
        }
        catch
        {
            foreach (var storageKey in savedKeys)
            {
                await privateFileStorage.DeleteAsync(
                    storageKey,
                    CancellationToken.None);
            }

            throw;
        }

        foreach (var oldDocument in oldDocuments)
        {
            try
            {
                await privateFileStorage.DeleteAsync(
                    oldDocument.StorageKey,
                    CancellationToken.None);
            }
            catch (Exception exception)
            {
                logger.LogWarning(
                    exception,
                    "Could not delete old verification file {StorageKey}.",
                    oldDocument.StorageKey);
            }
        }

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