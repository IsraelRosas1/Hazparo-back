using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Hazparo.Application.Account.Commands.UpdateAccount;

public sealed class UpdateAccountCommand : IRequest<IdentityResult>
{
    public string DisplayName { get; set; } = string.Empty;
}