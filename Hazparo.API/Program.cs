using Hazparo.Domain.Entities;
using Hazparo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString =
    builder.Configuration.GetConnectionString("HazparoDb")
    ?? throw new InvalidOperationException(
        "Connection string 'HazparoDb' was not found.");

builder.Services.AddDbContext<HazparoDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql =>
    {
        npgsql.UseNetTopologySuite();

        npgsql.MapEnum<UserRole>("user_role");
        npgsql.MapEnum<AccountStatus>("account_status");
        npgsql.MapEnum<VerificationStatus>("verification_status");
        npgsql.MapEnum<ServiceRequestStatus>("service_request_status");
        npgsql.MapEnum<QuoteStatus>("quote_status");
    })
    .UseSnakeCaseNamingConvention());

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
