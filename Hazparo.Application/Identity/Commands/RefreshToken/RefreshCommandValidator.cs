using FluentValidation;

namespace Hazparo.Application.Identity.Commands.RefreshCommand;

public sealed class RefreshCommandValidator
    : AbstractValidator<RefreshCommand>
{
    public RefreshCommandValidator()
    {
        RuleFor(command => command.RefreshToken)
            .NotEmpty();
    }
}