using CampusServicesPortal.Modules.Notifications.DTOs;
using FluentValidation;

namespace CampusServicesPortal.Modules.Notifications.Validators;

public sealed class CreateNotificationValidator
    : AbstractValidator<CreateNotificationDto>
{
    public CreateNotificationValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0);

        RuleFor(x => x.Type)
            .NotEmpty()
            .MaximumLength(80);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Message)
            .NotEmpty();
    }
}