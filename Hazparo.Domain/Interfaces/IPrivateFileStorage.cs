namespace Hazparo.Domain.Interfaces;

public interface IPrivateFileStorage
{
    Task<string> SaveAsync( Stream content, Guid professionalId, string documentType,
        CancellationToken cancellationToken);

    Task DeleteAsync(string storageKey, CancellationToken cancellationToken);
    Task<string> SaveServiceRequestPhotoAsync(Stream content,Guid requestId, CancellationToken cancellationToken);
}