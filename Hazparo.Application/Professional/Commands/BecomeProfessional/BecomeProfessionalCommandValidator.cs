using FluentValidation;

namespace Hazparo.Application.Professional.Commands.BecomeProfessional;

public sealed class BecomeProfessionalCommandValidator
    : AbstractValidator<BecomeProfessionalCommand>
{
    public BecomeProfessionalCommandValidator()
    {
        RuleFor(command => command.DisplayName)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(command => command.PhoneNumber)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(command => command.YearsExperience)
            .GreaterThanOrEqualTo((short)0);

        RuleFor(command => command.BusinessName)
            .MaximumLength(160);

        RuleFor(command => command.Bio)
            .MaximumLength(5000);
        RuleFor(command => command.ServiceRadiusKm)
            .InclusiveBetween((short)1, (short)100);

        RuleFor(command => command.HourlyRateMxn)
            .GreaterThanOrEqualTo(0)
            .When(command => command.HourlyRateMxn.HasValue);

        RuleFor(command => command.CategoryIds)
            .NotEmpty();
        RuleFor(command => command.Latitude)
            .InclusiveBetween(-90m, 90m);

        RuleFor(command => command.Longitude)
            .InclusiveBetween(-180m, 180m);

        RuleFor(command => command.AddressLine1)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.AddressLine2)
            .MaximumLength(200);

        RuleFor(command => command.Neighborhood)
            .MaximumLength(120);

        RuleFor(command => command.City)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(command => command.State)
            .MaximumLength(120);

        RuleFor(command => command.PostalCode)
            .MaximumLength(20);

        RuleFor(command => command.CountryCode)
            .NotEmpty()
            .Length(2);
    }
}