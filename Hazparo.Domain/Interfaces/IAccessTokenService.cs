using Hazparo.Domain.Entities;
using Hazparo.Domain.Models;

namespace Hazparo.Domain.Interfaces;

public interface IAccessTokenService
{
    Task<IssuedAccessToken> CreateAsync(
        ApplicationUser user);
}