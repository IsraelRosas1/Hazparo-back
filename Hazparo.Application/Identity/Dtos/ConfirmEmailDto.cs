namespace Hazparo.Application.Identity.Dtos;

public sealed class ConfirmEmailDto
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
}