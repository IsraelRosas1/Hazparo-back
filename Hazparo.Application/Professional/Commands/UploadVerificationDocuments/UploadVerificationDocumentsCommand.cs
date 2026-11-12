using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Hazparo.Application.Professional.Commands
    .UploadVerificationDocuments;

public sealed class UploadVerificationDocumentsCommand
    : IRequest<IdentityResult>
{
    public UploadedProfessionalFile FacePhoto { get; set; } = null!;
    public UploadedProfessionalFile IneFrontPhoto { get; set; } = null!;
    public UploadedProfessionalFile IneBackPhoto { get; set; } = null!;
}