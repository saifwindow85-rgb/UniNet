using Contracts.Requests.AcademicRequests.UniversityRequests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.AcademicValidators.UniversityValidators
{
    public class UpdateUniversityValidator : AbstractValidator<UpdateUniversityDTO>
    {
        UpdateUniversityValidator()
        {
            RuleFor(u => u.UniversityName)
            .Cascade(CascadeMode.StopOnFirstFailure) // Stops validation after the first failure
            .NotEmpty().WithMessage("University name is required.")
            .MaximumLength(250).WithMessage("University name cannot exceed 250 characters.")
            .MinimumLength(3).WithMessage("University name must be at least 3 characters long.")
            .Must(name => name.Trim().Length > 0).WithMessage("University name cannot be empty or whitespace.");

            // Description Validation
            // Even for "unlimited" text, it is professional to set a reasonable technical limit 
            // to prevent database bloat or memory issues.
            RuleFor(u => u.Description)
                .MaximumLength(5000).WithMessage("Description cannot exceed 5000 characters.")
                .When(u => !string.IsNullOrEmpty(u.Description));
        }
    }
}
