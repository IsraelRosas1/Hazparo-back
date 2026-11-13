using Hazparo.Domain.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Hazparo.Infrastructure.Storage;

public sealed class LocalPrivateFileStorage(
    IHostEnvironment environment)
    : IPrivateFileStorage
{
    private readonly string rootPath =
        Path.Combine(
            environment.ContentRootPath,
            "PrivateFiles");

    public async Task<string> SaveAsync(
        Stream content,
        Guid professionalId,
        string documentType,
        CancellationToken cancellationToken)
    {
        var fileName = $"{Guid.NewGuid():N}.bin";

        var relativeKey =
            Path.Combine(
                "professional-documents",
                professionalId.ToString(),
                documentType,
                fileName);

        var fullPath = Path.Combine(
            rootPath,
            relativeKey);

        var directory = Path.GetDirectoryName(fullPath)!;

        Directory.CreateDirectory(directory);

        await using var fileStream = new FileStream(
            fullPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        await content.CopyToAsync(
            fileStream,
            cancellationToken);

        return relativeKey;
    }

    public Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(
            rootPath,
            storageKey);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
    public async Task<string> SaveServiceRequestPhotoAsync(
    Stream content,
    Guid requestId,
    CancellationToken cancellationToken)
    {
        var fileName = $"{Guid.NewGuid():N}.bin";

        var relativeKey = Path.Combine(
            "service-request-photos",
            requestId.ToString(),
            fileName);

        var fullPath = Path.Combine(
            rootPath,
            relativeKey);

        var directory = Path.GetDirectoryName(fullPath)!;

        Directory.CreateDirectory(directory);

        await using var fileStream = new FileStream(
            fullPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        await content.CopyToAsync(
            fileStream,
            cancellationToken);

        return relativeKey;
    }
}