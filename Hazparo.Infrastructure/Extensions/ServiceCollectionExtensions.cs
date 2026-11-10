using Hazparo.Domain.Entities;
using Hazparo.Domain.Interfaces;
using Hazparo.Domain.Repositories;
using Hazparo.Infrastructure.Email;
using Hazparo.Infrastructure.Identity;
using Hazparo.Infrastructure.Persistence;
using Hazparo.Infrastructure.Persistence.Repositories;
using Hazparo.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hazparo.Infrastructure.Extensions;
public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //Already registered identity in AuthenticationServiceExtensions.cs
        // services.AddIdentityApiEndpoints<ApplicationUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<HazparoDbContext>();
        
        //Repositories
        //sevices.AddScoped<IRepositry,Repository>();
        var connectionString = configuration.GetConnectionString("HazparoDb");

        services.AddDbContext<HazparoDbContext>(options =>
        options.UseNpgsql(connectionString, npgsql =>
        {
            npgsql.UseNetTopologySuite();

            npgsql.MapEnum<AccountStatus>("account_status");
            npgsql.MapEnum<VerificationStatus>("verification_status");
            npgsql.MapEnum<ServiceRequestStatus>("service_request_status");
            npgsql.MapEnum<QuoteStatus>("quote_status");
        })
        .UseSnakeCaseNamingConvention());

        services.AddScoped<IHazparoSeeder, HazparoSeeder>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IAccessTokenService, JwtTokenService>();
        services.AddScoped<IAccountEmailSender, AccountEmailSender>();

    }
}