using FluentValidation;

namespace Hazparo.Application.Account.Commands.UpdateAccount;

public sealed class UpdateAccountCommandValidator
    : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator()
    {
        RuleFor(command => command.DisplayName)
            .NotEmpty()
            .MaximumLength(120);
    }
}