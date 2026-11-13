using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hazparo.API.Extensions;

public static class CorsServiceExtensions
{
    public const string ExpoDevelopmentPolicy =
        "ExpoDevelopment";

    public static IServiceCollection AddHazparoCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .GetChildren()
            .Select(section => section.Value)
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .ToArray();

        services.AddCors(options =>
        {
            options.AddPolicy(
                ExpoDevelopmentPolicy,
                policy =>
                {
                    policy
                        .WithOrigins(
                        "http://localhost:5173",
                        "http://10.0.0.60:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        return services;
    }
}
