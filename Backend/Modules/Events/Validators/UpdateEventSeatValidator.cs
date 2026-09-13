using CampusServicesPortal.Modules.Events.DTOs.EventSeats;
using FluentValidation;

namespace CampusServicesPortal.Modules.Events.Validators.EventSeats;

public sealed class UpdateEventSeatValidator
    : AbstractValidator<UpdateEventSeatDto>
{
    public UpdateEventSeatValidator()
    {
        RuleFor(x => x.SeatNumber)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.SectionName)
            .MaximumLength(100);

        RuleFor(x => x.RowLabel)
            .MaximumLength(20);
    }
}