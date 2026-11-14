using FluentValidation;

namespace Hazparo.Application.ServiceRequests.Commands.CancelServiceRequest;

public sealed class CancelServiceRequestCommandValidator
    : AbstractValidator<CancelServiceRequestCommand>
{
    public CancelServiceRequestCommandValidator()
    {
        RuleFor(command => command.RequestId)
            .NotEmpty();

        RuleFor(command => command.Reason)
            .NotEmpty()
            .MaximumLength(1000);
    }
}