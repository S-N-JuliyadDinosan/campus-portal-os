using FluentValidation;
using CampusServicesPortal.Modules.Complaints.DTOs;

namespace CampusServicesPortal.Modules.Complaints.Validators;

public sealed class ComplaintCategoryUpdateValidator
    : AbstractValidator<ComplaintCategoryUpdateDto>
{
    public ComplaintCategoryUpdateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(100)
            .WithMessage("Category name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");
    }
}