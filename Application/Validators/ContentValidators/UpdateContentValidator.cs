using Contracts.Requests.ContentRequests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ContentValidators
{
    public class UpdateContentValidator : AbstractValidator<UpdateContentDTO>
    {
        public UpdateContentValidator()
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(500).WithMessage("Title must not exceed 500 characters.");

            RuleFor(c => c.Body)
                .NotEmpty().WithMessage("Body is required.");
        }
    }
}
