using CampusServicesPortal.Modules.Events.DTOs.EventRegistrations;
using FluentValidation;

namespace CampusServicesPortal.Modules.Events.Validators.EventRegistrations;

public sealed class CreateEventRegistrationValidator
    : AbstractValidator<CreateEventRegistrationDto>
{
    public CreateEventRegistrationValidator()
    {
        RuleFor(x => x.EventId)
            .GreaterThan(0);

        RuleFor(x => x.EventSeatId)
            .GreaterThan(0)
            .When(x => x.EventSeatId.HasValue);
    }
}