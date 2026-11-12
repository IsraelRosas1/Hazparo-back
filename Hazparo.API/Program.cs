using Hazparo.Domain.Entities;
using Hazparo.API.Extensions;
using Hazparo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Hazparo.Infrastructure.Extensions;
using Hazparo.Application.Extensions;
using Hazparo.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);
const string ViteCorsPolicy = "ViteCorsPolicy";

builder.AddPresentation();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHazparoAuthentication(
    builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddHazparoRateLimiting();

builder.Services.AddCors(options =>
{
    options.AddPolicy(ViteCorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://10.0.0.60:5173",
                "http://35.16.112.140:5173",
                "http://192.168.1.252:5173",
                "http://localhost:5174",
                "http://10.0.0.60:5174",
                "http://35.16.112.140:5174",
                "http://192.168.1.252:5173",
                "http://35.16.106.252:5173/"
                
                )
            .AllowAnyHeader()
            .AllowAnyMethod();    
    });
});
var app = builder.Build();
var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<IHazparoSeeder>();
await seeder.SeedAsync();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(ViteCorsPolicy);

// app.MapGroup("api/identity").MapIdentityApi<ApplicationUser>();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseRouting();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
