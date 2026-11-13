using FluentValidation;

namespace Hazparo.Application.ServiceRequests.Commands
    .CreateServiceRequest;

public sealed class CreateServiceRequestCommandValidator: AbstractValidator<CreateServiceRequestCommand>
{
    public CreateServiceRequestCommandValidator()
    {
        RuleFor(command => command.CategoryId)
            .NotEmpty();

        RuleFor(command => command.Title)
            .MaximumLength(140);

        RuleFor(command => command.Description)
            .NotEmpty()
            .MaximumLength(5000);

        RuleFor(command => command.AddressLine1)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.City)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(command => command.CountryCode)
            .NotEmpty()
            .Length(2);

        RuleFor(command => command.LocationReference)
            .MaximumLength(500);

        RuleFor(command => command.Latitude)
            .InclusiveBetween(-90m, 90m);

        RuleFor(command => command.Longitude)
            .InclusiveBetween(-180m, 180m);

        RuleFor(command => command.Photos)
            .Must(photos => photos.Count <= 8)
            .WithMessage("A service request can contain up to 8 photos.");
    }
}