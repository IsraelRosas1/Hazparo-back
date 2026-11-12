using FluentValidation;

namespace Hazparo.Application.Professional.Commands.BecomeProfessional;

public sealed class BecomeProfessionalCommandValidator
    : AbstractValidator<BecomeProfessionalCommand>
{
    public BecomeProfessionalCommandValidator()
    {
        RuleFor(command => command.ServiceRadiusKm)
            .InclusiveBetween((short)1, (short)100);

        RuleFor(command => command.YearsExperience)
            .GreaterThanOrEqualTo((short)0);

        RuleFor(command => command.HourlyRateMxn)
            .GreaterThanOrEqualTo(0)
            .When(command => command.HourlyRateMxn.HasValue);

        RuleFor(command => command.CategoryIds)
            .NotEmpty();

        RuleForEach(command => command.CategoryIds)
            .NotEmpty();

        RuleFor(command => command.Latitude)
            .InclusiveBetween(-90m, 90m);

        RuleFor(command => command.Longitude)
            .InclusiveBetween(-180m, 180m);

        RuleFor(command => command.City)
            .MaximumLength(120);
    }
}