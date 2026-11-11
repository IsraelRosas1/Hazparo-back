using Hazparo.Domain.Entities;

namespace Hazparo.Domain.Repositories;
public interface IIdentityUserService
{
    Task<ApplicationUser?> FindByEmailAsync(string email, CancellationToken cancellationToken);
    Task<ApplicationUser?> FindByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<IdentityOperationResult> CreateAsync(ApplicationUser user, string password, CancellationToken cancellationToken);
    Task<IdentityOperationResult> AddToRoleAsync(ApplicationUser user, string role, CancellationToken cancellationToken);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password, CancellationToken cancellationToken);
    Task<IdentityOperationResult> AccessFailedAsync(ApplicationUser user, CancellationToken cancellationToken);
    Task<IdentityOperationResult> ResetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken);
    Task<IdentityOperationResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken);
    Task<IReadOnlyList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken);
}

public sealed record IdentityOperationResult(bool Succeeded, string ErrorMessage);
