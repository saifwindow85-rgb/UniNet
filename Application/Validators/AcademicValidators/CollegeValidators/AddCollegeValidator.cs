using Contracts.Requests.AcademicRequests.CollegeRequests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.AcademicValidators.CollegeValidators
{
    public class AddCollegeValidator : AbstractValidator<AddCollegeDTO>
    {
        public AddCollegeValidator()
        {
            // College Name Validation
            RuleFor(c => c.CollegeName)
                .Cascade(CascadeMode.StopOnFirstFailure) // Stops validation after the first failure
                .NotEmpty().WithMessage("College name is required.")
                .MaximumLength(250).WithMessage("College name cannot exceed 250 characters.")
                .MinimumLength(3).WithMessage("College name must be at least 3 characters long.")
                .Must(name => name.Trim().Length > 0).WithMessage("College name cannot be empty or whitespace.");

            // Description Validation
            // Even for "unlimited" text, it is professional to set a reasonable technical limit 
            // to prevent database bloat or memory issues.
            RuleFor(c => c.Description)
                .MaximumLength(5000).WithMessage("Description cannot exceed 5000 characters.")
                .When(c => !string.IsNullOrEmpty(c.Description));
        }
    }
}
