namespace Hazparo.Domain.Interfaces;

public interface IAccountEmailSender
{
    Task SendConfirmationAsync(
        string email,
        Guid userId,
        string encodedToken);

    Task SendPasswordResetAsync(
        string email,
        string encodedToken);
}