using Hazparo.Domain.Constants;
using Hazparo.Domain.Entities;
using Hazparo.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Hazparo.Application.Identity.Commands.Register;

public sealed class RegisterCommandHandler(
    UserManager<ApplicationUser> userManager,
    IAccountEmailSender accountEmailSender)
    : IRequestHandler<RegisterCommand, IdentityResult>
{
    public async Task<IdentityResult> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser =
            await userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "DuplicateEmail",
                Description = "An account already exists for this email."
            });
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName,
            Status = AccountStatus.Pending
        };

        var createResult = await userManager.CreateAsync(
            user,
            request.Password);

        if (!createResult.Succeeded)
        {
            return createResult;
        }

        var roleResult = await userManager.AddToRoleAsync(
            user,
            UserRoles.Client);

        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return roleResult;
        }

        var token =
            await userManager.GenerateEmailConfirmationTokenAsync(user);

        var encodedToken = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(token));

        await accountEmailSender.SendConfirmationAsync(
            user.Email!,
            user.Id,
            encodedToken);

        return IdentityResult.Success;
    }
}