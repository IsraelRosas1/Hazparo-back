namespace Hazparo.Application.Professional.Dtos;

public sealed class ProfessionalVerificationStatusDto
{
    public string Status { get; set; } = string.Empty;

    public bool IsSubmitted { get; set; }
    public bool IsPending { get; set; }
    public bool IsVerificationComplete { get; set; }
    public bool IsRejected { get; set; }
}