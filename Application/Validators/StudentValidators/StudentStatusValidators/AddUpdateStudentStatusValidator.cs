using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Requests.StudentRequests;

namespace Application.Validators.StudentValidators.StudentStatusValidators
{
    public class AddUpdateStudentStatusValidator : AbstractValidator<AddUpdateStudentStatusDTO>
    {
        public AddUpdateStudentStatusValidator()
        {
            // Validations for Name
            RuleFor(s => s.Name)
                .NotEmpty()
                    .WithMessage("Status name is required.")
                .MaximumLength(50)
                    .WithMessage("Status name cannot exceed 50 characters.")
                .Must(name => string.IsNullOrWhiteSpace(name) || name.Trim() == name)
                    .WithMessage("Status name should not contain leading or trailing spaces.");

            // Validations for Description
            RuleFor(s => s.Description)
                .MaximumLength(300)
                    .WithMessage("Description cannot exceed 300 characters.")
                .When(s => !string.IsNullOrEmpty(s.Description));
        }
    }
}
