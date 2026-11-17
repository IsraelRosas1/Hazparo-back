using FluentValidation;

namespace Hazparo.Application.Professional.Commands
    .UpdateProfessionalProfile;

public sealed class UpdateProfessionalProfileCommandValidator: AbstractValidator<UpdateProfessionalProfileCommand>
{
    public UpdateProfessionalProfileCommandValidator()
    {
        RuleFor(command => command.DisplayName)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(command => command.BusinessName)
            .MaximumLength(160);

        RuleFor(command => command.Bio)
            .MaximumLength(5000);

        RuleFor(command => command.HourlyRateMxn)
            .GreaterThanOrEqualTo(0)
            .When(command => command.HourlyRateMxn.HasValue);

        RuleFor(command => command.ServiceRadiusKm)
            .InclusiveBetween((short)1, (short)100);
    }
}