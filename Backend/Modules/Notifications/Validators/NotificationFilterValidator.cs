using CampusServicesPortal.Modules.Notifications.DTOs;
using FluentValidation;

namespace CampusServicesPortal.Modules.Notifications.Validators;

public sealed class NotificationFilterValidator
    : AbstractValidator<NotificationFilterDto>
{
    public NotificationFilterValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);
    }
}