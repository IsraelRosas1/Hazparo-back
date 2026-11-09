using Hazparo.Domain.Entities;
using Hazparo.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hazparo.Infrastructure.Extensions;
public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddIdentityApiEndpoints<ApplicationUser>().AddEntityFrameworkStores<HazparoDbContext>();
        //Repositories
        //sevices.AddScoped<IRepositry,Repository>();
    }
}