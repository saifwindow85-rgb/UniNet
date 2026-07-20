using Contracts.Requests.AcademicRequests.CollegeRequests;
using Contracts.Requests.AcademicRequests.DepartmentRequests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.AcademicValidators.DepartmentValidators
{
    public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentDTO>
    {
        public UpdateDepartmentValidator() 
        {
            // Department Name Validation
            RuleFor(d => d.DepartmentName)
                .Cascade(CascadeMode.StopOnFirstFailure) // Stops validation after the first failure
                .NotEmpty().WithMessage("Department name is required.")
                .MaximumLength(250).WithMessage("Department name cannot exceed 250 characters.")
                .MinimumLength(3).WithMessage("Department name must be at least 3 characters long.")
                .Must(name => name.Trim().Length > 0).WithMessage("Department name cannot be empty or whitespace.");

            // Description Validation
            // Even for "unlimited" text, it is professional to set a reasonable technical limit 
            // to prevent database bloat or memory issues.
            RuleFor(c => c.Description)
                .MaximumLength(5000).WithMessage("Description cannot exceed 5000 characters.")
                .When(c => !string.IsNullOrEmpty(c.Description));
        }
    }
}
