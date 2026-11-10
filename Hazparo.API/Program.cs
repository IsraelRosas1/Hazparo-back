using Hazparo.Domain.Entities;
using Hazparo.API.Extensions;
using Hazparo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Hazparo.Infrastructure.Extensions;
using Hazparo.Application.Extensions;
using Hazparo.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.AddPresentation();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHazparoAuthentication(
    builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();


var app = builder.Build();
var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<IHazparoSeeder>();
await seeder.SeedAsync();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
// app.MapGroup("api/identity").MapIdentityApi<ApplicationUser>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
