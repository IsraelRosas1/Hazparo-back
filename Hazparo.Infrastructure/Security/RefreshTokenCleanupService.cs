using Hazparo.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hazparo.Infrastructure.Security;

public sealed class RefreshTokenCleanupService(IServiceScopeFactory scopeFactory,
    ILogger<RefreshTokenCleanupService> logger): BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await CleanupAsync(stoppingToken);

            using var timer =new PeriodicTimer(TimeSpan.FromHours(24));

            while (await timer.WaitForNextTickAsync(
                stoppingToken))
            {
                await CleanupAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
        }
    }

    private async Task CleanupAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            using var scope =
                scopeFactory.CreateScope();

            var repository =
                scope.ServiceProvider
                    .GetRequiredService<IRefreshTokenRepository>();

            var deleted =
                await repository.DeleteExpiredAsync(DateTimeOffset.UtcNow,cancellationToken);

            logger.LogInformation(
                "Deleted {Count} expired refresh tokens.",
                deleted);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Could not clean expired refresh tokens.");
        }
    }
}