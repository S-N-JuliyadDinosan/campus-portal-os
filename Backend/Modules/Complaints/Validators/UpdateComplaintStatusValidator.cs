using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Modules.Complaints.DTOs;
using FluentValidation;

namespace CampusServicesPortal.Modules.Complaints.Validators;

public sealed class UpdateComplaintStatusValidator
    : AbstractValidator<UpdateComplaintStatusDto>
{
    public UpdateComplaintStatusValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid complaint status.");

        RuleFor(x => x.ResolutionNote)
            .MaximumLength(2000)
            .WithMessage("Resolution note must not exceed 2000 characters.");

        RuleFor(x => x.ResolutionNote)
            .NotEmpty()
            .When(x => x.Status == ComplaintStatus.Resolved)
            .WithMessage(
                "Resolution note is required when resolving a complaint.");
    }
}