namespace Hazparo.Application.Identity.Dtos;

public sealed class LoginResultDto
{
    public bool Succeeded { get; set; }
    public bool IsLockedOut { get; set; }
    public bool RequiresConfirmedEmail { get; set; }
    public string? Error { get; set; }
    public AuthenticationTokensDto? Tokens { get; set; }
}