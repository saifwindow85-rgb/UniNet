using Contracts.Requests.AcademicRequests.SectionRequests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.AcademicValidators.SectionValidators
{
    public class UpdateSectionValidator : AbstractValidator<UpdateSectionDTO>
    {
        public UpdateSectionValidator() 
        {
            // Section Name Validation
            RuleFor(s => s.SectionName)
                .Cascade(CascadeMode.StopOnFirstFailure) // Stops validation after the first failure
                .NotEmpty().WithMessage("Section name is required.")
                .MaximumLength(250).WithMessage("Section name cannot exceed 250 characters.")
                .MinimumLength(3).WithMessage("Section name must be at least 3 characters long.")
                .Must(name => name.Trim().Length > 0).WithMessage("Section name cannot be empty or whitespace.");
        }
    }
}
