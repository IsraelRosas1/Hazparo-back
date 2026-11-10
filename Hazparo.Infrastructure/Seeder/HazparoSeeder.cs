using Hazparo.Domain.Constants;
using Hazparo.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Hazparo.Infrastructure.Identity;

public sealed class HazparoSeeder(
    RoleManager<IdentityRole<Guid>> roleManager,
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration) : IHazparoSeeder
{
    public async Task SeedAsync()
    {
        var roles = new[]
        {
            UserRoles.Client,
            UserRoles.Professional,
            UserRoles.Admin
        };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole<Guid>
                    {
                        Id = Guid.NewGuid(),
                        Name = roleName
                    });

                EnsureSucceeded(
                    result,
                    $"Could not create role '{roleName}'.");
            }
        }

        var adminEmail =
            configuration["IdentitySeed:AdminEmail"];

        var adminPassword =
            configuration["IdentitySeed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var admin =
            await userManager.FindByEmailAsync(adminEmail);

        if (admin is null)
        {
            admin = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = adminEmail,
                Email = adminEmail,
                DisplayName = "Hazparo Administrator",
                Status = AccountStatus.Active,
                EmailConfirmed = true,
                EmailConfirmedAt = DateTimeOffset.UtcNow
            };

            var result = await userManager.CreateAsync(
                admin,
                adminPassword);

            EnsureSucceeded(
                result,
                "Could not create the admin user.");
        }

        if (!await userManager.IsInRoleAsync(
                admin,
                UserRoles.Admin))
        {
            var result = await userManager.AddToRoleAsync(
                admin,
                UserRoles.Admin);

            EnsureSucceeded(
                result,
                "Could not assign the admin role.");
        }
    }

    private static void EnsureSucceeded(
        IdentityResult result,
        string message)
    {
        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(error =>
                    error.Description));

            throw new InvalidOperationException(
                $"{message} {errors}");
        }
    }
}