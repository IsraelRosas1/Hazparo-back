namespace Hazparo.Application.Professional.Commands
    .UploadVerificationDocuments;

public sealed class UploadedProfessionalFile
{
    public Stream Content { get; set; } = null!;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Length { get; set; }
}