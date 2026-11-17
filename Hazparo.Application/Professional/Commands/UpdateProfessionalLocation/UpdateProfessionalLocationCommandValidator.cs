using FluentValidation;

public sealed class UpdateProfessionalLocationCommandValidator: AbstractValidator<UpdateProfessionalLocationCommand>
{
    public UpdateProfessionalLocationCommandValidator()
    {
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

        RuleFor(command => command.Latitude)
            .InclusiveBetween(-90m, 90m);

        RuleFor(command => command.Longitude)
            .InclusiveBetween(-180m, 180m);
    }
}