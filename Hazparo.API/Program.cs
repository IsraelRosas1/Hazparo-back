using Hazparo.Domain.Entities;
using Hazparo.API.Extensions;
using Hazparo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddPresentation();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHazparoAuthentication(
    builder.Configuration);

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

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapGroup("api/identity").MapIdentityApi<ApplicationUser>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
