using Hazparo.Domain.Entities;
using Hazparo.API.Controllers;
using Microsoft.EntityFrameworkCore;
using Hazparo.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<HazparoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HazparoDb")));

var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
