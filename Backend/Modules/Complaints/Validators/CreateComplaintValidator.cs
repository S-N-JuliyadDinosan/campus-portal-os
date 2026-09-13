using FluentValidation;
using CampusServicesPortal.Modules.Complaints.DTOs;

namespace CampusServicesPortal.Modules.Complaints.Validators;

public sealed class CreateComplaintValidator
    : AbstractValidator<CreateComplaintDto>
{
    public CreateComplaintValidator()
    {
        RuleFor(x => x.ComplaintCategoryId)
            .GreaterThan(0)
            .WithMessage("A valid complaint category is required.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Complaint description is required.")
            .MaximumLength(2000)
            .WithMessage("Complaint description must not exceed 2000 characters.");
    }
}