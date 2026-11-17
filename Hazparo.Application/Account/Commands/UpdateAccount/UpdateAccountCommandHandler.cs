using Hazparo.Application.User;
using Hazparo.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Hazparo.Application.Account.Commands.UpdateAccount;

public sealed class UpdateAccountCommandHandler(
    IUserContext userContext,
    UserManager<ApplicationUser> userManager)
    : IRequestHandler<UpdateAccountCommand, IdentityResult>
{
    public async Task<IdentityResult> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (currentUser is null ||
            !Guid.TryParse(currentUser.Id, out var userId))
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "Unauthorized",
                    Description = "The authenticated user is invalid."
                });
        }

        var user = await userManager.FindByIdAsync(
            userId.ToString());

        if (user is null)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "The user was not found."
                });
        }

        user.DisplayName = request.DisplayName.Trim();

        return await userManager.UpdateAsync(user);
    }
}