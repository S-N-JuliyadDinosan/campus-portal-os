using CampusServicesPortal.Modules.SystemSettings.DTOs;
using FluentValidation;

namespace CampusServicesPortal.Modules.SystemSettings.Validators;

public sealed class ReservationHoldMinutesValidator
    : AbstractValidator<ReservationHoldMinutesDto>
{
    public ReservationHoldMinutesValidator()
    {
        RuleFor(x => x.Minutes)
            .NotNull();
    }
}