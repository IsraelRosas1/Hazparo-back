using FluentValidation;

namespace Hazparo.Application.Identity.Commands.Register;

public sealed class RegisterCommandValidator
    : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(command => command.Password)
            .NotEmpty();

        RuleFor(command => command.DisplayName)
            .NotEmpty()
            .MaximumLength(120);
    }
}