using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace Hazparo.API.Extensions;

public static class RateLimitingServiceExtensions
{
    public static IServiceCollection AddHazparoRateLimiting(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.ContentType =
                    "application/json";

                await context.HttpContext.Response.WriteAsJsonAsync(
                    new
                    {
                        message =
                            "Too many requests. Please try again later."
                    },
                    cancellationToken);
            };

            options.AddPolicy(
                "identity-login",
                httpContext => CreateFixedWindowLimiter(
                    httpContext,
                    permitLimit: 5,
                    window: TimeSpan.FromMinutes(15)));

            options.AddPolicy(
                "identity-refresh",
                httpContext => CreateFixedWindowLimiter(
                    httpContext,
                    permitLimit: 30,
                    window: TimeSpan.FromMinutes(1)));

            options.AddPolicy(
                "identity-forgot-password",
                httpContext => CreateFixedWindowLimiter(
                    httpContext,
                    permitLimit: 3,
                    window: TimeSpan.FromMinutes(15)));

            options.AddPolicy(
                "identity-reset-password",
                httpContext => CreateFixedWindowLimiter(
                    httpContext,
                    permitLimit: 5,
                    window: TimeSpan.FromMinutes(15)));
        });

        return services;
    }

    private static RateLimitPartition<string> CreateFixedWindowLimiter(
        HttpContext httpContext,
        int permitLimit,
        TimeSpan window)
    {
        var clientIp =
            httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: clientIp,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = window,
                QueueLimit = 0,
                AutoReplenishment = true
            });
    }
}