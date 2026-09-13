using CampusServicesPortal.Modules.SystemSettings.DTOs;
using FluentValidation;

namespace CampusServicesPortal.Modules.SystemSettings.Validators;

public sealed class UpdateSystemSettingValidator
    : AbstractValidator<UpdateSystemSettingDto>
{
    public UpdateSystemSettingValidator()
    {
        RuleFor(x => x.SettingValue)
            .NotEmpty()
            .MaximumLength(1000);
    }
}