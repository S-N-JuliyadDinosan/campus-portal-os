using CampusServicesPortal.Modules.Events.DTOs.Events;
using FluentValidation;

namespace CampusServicesPortal.Modules.Events.Validators.Events;

public sealed class UpdateEventValidator
    : AbstractValidator<UpdateEventDto>
{
    public UpdateEventValidator()
    {
        RuleFor(x => x.VenueId)
            .GreaterThan(0);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.EndAt)
            .GreaterThan(x => x.StartAt)
            .WithMessage("End time must be after start time.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0);
    }
}