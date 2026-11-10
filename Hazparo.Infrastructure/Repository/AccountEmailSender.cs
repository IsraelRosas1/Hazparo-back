using Hazparo.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hazparo.Infrastructure.Email;

public sealed class AccountEmailSender(
    IConfiguration configuration,
    ILogger<AccountEmailSender> logger)
    : IAccountEmailSender
{
    public Task SendConfirmationAsync(
        string email,
        Guid userId,
        string encodedToken)
    {
        var clientBaseUrl =
            configuration["ClientApp:BaseUrl"]
            ?? "http://localhost:5173";

        var link =
            $"{clientBaseUrl}/confirm-email" +
            $"?userId={userId}" +
            $"&token={Uri.EscapeDataString(encodedToken)}";

        logger.LogWarning(
            "Development confirmation link for {Email}: {Link}",
            email,
            link);

        return Task.CompletedTask;
    }

    public Task SendPasswordResetAsync(
        string email,
        string encodedToken)
    {
        var clientBaseUrl =
            configuration["ClientApp:BaseUrl"]
            ?? "http://localhost:5173";

        var link =
            $"{clientBaseUrl}/reset-password" +
            $"?email={Uri.EscapeDataString(email)}" +
            $"&token={Uri.EscapeDataString(encodedToken)}";

        logger.LogWarning(
            "Development password-reset link for {Email}: {Link}",
            email,
            link);

        return Task.CompletedTask;
    }
}