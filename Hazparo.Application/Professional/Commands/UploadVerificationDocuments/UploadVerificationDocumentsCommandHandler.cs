using Hazparo.Application.User;
using Hazparo.Domain.Entities;
using Hazparo.Domain.Interfaces;
using Hazparo.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Hazparo.Application.Professional.Commands
    .UploadVerificationDocuments;

public sealed class UploadVerificationDocumentsCommandHandler(
    IUserContext userContext,
    IPrivateFileStorage privateFileStorage,
    IProfessionalDocumentRepository documentRepository)
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
            !Guid.TryParse(currentUser.Id, out var professionalId))
        {
            return Failed(
                "Unauthorized",
                "The authenticated user could not be identified.");
        }

        var isProfessional =
            await documentRepository.ProfessionalExistsAsync(
                professionalId,
                cancellationToken);

        if (!isProfessional)
        {
            return Failed(
                "ProfessionalProfileNotFound",
                "Complete professional onboarding first.");
        }

        var documentsAlreadyExist =
            await documentRepository.DocumentsAlreadyExistAsync(
                professionalId,
                cancellationToken);

        if (documentsAlreadyExist)
        {
            return Failed(
                "DocumentsAlreadyUploaded",
                "Verification documents have already been uploaded.");
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
                    "Only JPEG and PNG images are accepted.");
            }
        }

        var savedKeys = new List<string>();
        var documents = new List<
            ProfessionalVerificationDocument>();

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

                documents.Add(
                    new ProfessionalVerificationDocument
                    {
                        Id = Guid.NewGuid(),
                        ProfessionalId = professionalId,
                        DocumentType = item.Type,
                        StorageKey = storageKey,
                        OriginalFileName = Path.GetFileName(
                            item.File.OriginalFileName),
                        ContentType = item.File.ContentType,
                        SizeBytes = item.File.Length,
                        CreatedAt = DateTimeOffset.UtcNow
                    });
            }

            await documentRepository.SaveAsync(
                documents,
                cancellationToken);

            return IdentityResult.Success;
        }
        catch
        {
            foreach (var storageKey in savedKeys)
            {
                await privateFileStorage.DeleteAsync(
                    storageKey,
                    cancellationToken);
            }

            throw;
        }
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