using CampusServicesPortal.Modules.Events.DTOs.Venues;
using FluentValidation;

namespace CampusServicesPortal.Modules.Events.Validators.Venues;

public sealed class CreateVenueValidator
    : AbstractValidator<CreateVenueDto>
{
    public CreateVenueValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Location)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.Capacity)
            .GreaterThan(0);
    }
}