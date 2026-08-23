using Contracts.Requests.StudyRequestes.SubjectRequests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.StudyValidators.SubjectValidators
{
    public class AddSubjectValidator : AbstractValidator<AddSubjectDTO>
    {
        public AddSubjectValidator()
        {
            // Code Validation (Format: 3 letters, hyphen, 7 numbers -> e.g., abs-0000000)
            RuleFor(s => s.Code)
              .NotEmpty().WithMessage("Subject code is required.")
                   .MaximumLength(25).WithMessage("Subject code must not exceed 25 characters.")
                        .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Subject code must contain only letters and numbers.");

            // Name Validation
            RuleFor(s => s.Name)
                .NotEmpty().WithMessage("Subject name is required.")
                .MinimumLength(2).WithMessage("Subject name must be at least 2 characters long.")
                .MaximumLength(200).WithMessage("Subject name must not exceed 200 characters.");

            // Hours Validations
            RuleFor(s => s.CreditHours).GreaterThan(0).WithMessage("Credit hours must be greater than 0!");


            // Description Validation (Optional)
            RuleFor(s => s.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        }
    }
}
