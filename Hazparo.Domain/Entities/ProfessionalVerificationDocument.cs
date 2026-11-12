namespace Hazparo.Domain.Entities;

public class ProfessionalVerificationDocument
{
    public Guid Id { get; set; }

    public Guid ProfessionalId { get; set; }

    public ProfessionalDocumentType DocumentType { get; set; }

    public string StorageKey { get; set; } = null!;

    public string OriginalFileName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public long SizeBytes { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public ProfessionalProfile Professional { get; set; } = null!;
}