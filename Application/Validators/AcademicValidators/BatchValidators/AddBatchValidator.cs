using Contracts.Requests.AcademicRequests.BatchRequests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.AcademicValidators.BatchValidators
{
    public class AddBatchValidator : AbstractValidator<AddBatchDTO>
    {
        public AddBatchValidator() 
        {
            // Batch Name Validation
            RuleFor(b => b.BatchName)
                .Cascade(CascadeMode.StopOnFirstFailure) // Stops validation after the first failure
                .NotEmpty().WithMessage("Batch name is required.")
                .MaximumLength(250).WithMessage("Batch name cannot exceed 250 characters.")
                .MinimumLength(3).WithMessage("Batch name must be at least 3 characters long.")
                .Must(name => name.Trim().Length > 0).WithMessage("Batch name cannot be empty or whitespace.");

            //BatchYear Validation
            RuleFor(b => b.BatchYear)
                .InclusiveBetween(2000, 2026).WithMessage("BatchYear must be between 2000 & 2026 !");
            // TODO: Replace the upper bound with DateTime.UtcNow.Year before production.


            // Description Validation
            // Even for "unlimited" text, it is professional to set a reasonable technical limit 
            // to prevent database bloat or memory issues.
            RuleFor(b => b.Description)
                .MaximumLength(5000).WithMessage("Description cannot exceed 5000 characters.")
                .When(c => !string.IsNullOrEmpty(c.Description));
        }
    }
}
