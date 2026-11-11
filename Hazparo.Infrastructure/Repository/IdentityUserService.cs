using Hazparo.Domain.Entities;
using Hazparo.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
namespace Hazparo.Infrastructure.Repository;
public sealed class IdentityUserService(UserManager<ApplicationUser> userManager) : IIdentityUserService
{
    public async Task<ApplicationUser?> FindByEmailAsync(string email, CancellationToken cancellationToken) =>
        await userManager.FindByEmailAsync(email);

    public async Task<ApplicationUser?> FindByIdAsync(Guid userId, CancellationToken cancellationToken) =>
        await userManager.FindByIdAsync(userId.ToString());

    public Task<IdentityOperationResult> CreateAsync(
        ApplicationUser user,
        string password,
        CancellationToken cancellationToken) =>
        MapAsync(userManager.CreateAsync(user, password));

    public Task<IdentityOperationResult> AddToRoleAsync(
        ApplicationUser user,
        string role,
        CancellationToken cancellationToken) =>
        MapAsync(userManager.AddToRoleAsync(user, role));

    public Task<bool> CheckPasswordAsync(
        ApplicationUser user,
        string password,
        CancellationToken cancellationToken) =>
        userManager.CheckPasswordAsync(user, password);

    public Task<IdentityOperationResult> AccessFailedAsync(
        ApplicationUser user,
        CancellationToken cancellationToken) =>
        MapAsync(userManager.AccessFailedAsync(user));

    public Task<IdentityOperationResult> ResetAccessFailedCountAsync(
        ApplicationUser user,
        CancellationToken cancellationToken) =>
        MapAsync(userManager.ResetAccessFailedCountAsync(user));

    public Task<IdentityOperationResult> UpdateAsync(
        ApplicationUser user,
        CancellationToken cancellationToken) =>
        MapAsync(userManager.UpdateAsync(user));

    public async Task<IReadOnlyList<string>> GetRolesAsync(
        ApplicationUser user,
        CancellationToken cancellationToken) =>
        (await userManager.GetRolesAsync(user)).ToList();

    private static async Task<IdentityOperationResult> MapAsync(Task<IdentityResult> operation)
    {
        var result = await operation;
        return new IdentityOperationResult(
            result.Succeeded,
            string.Join(" ", result.Errors.Select(error => error.Description)));
    }
}