using Contracts.Requests.IdentityRequests.RoleRequests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.IdentityValidators.RoleValidators
{
    public class AddRoleValidator : AbstractValidator<AddRoleDTO>
    {
        public AddRoleValidator() 
        {
            RuleFor(r => r.RoleName)
            .MinimumLength(4).WithMessage("Minimum length is 4 letters!")
            .MaximumLength(30).WithMessage("Maximum length is 30 letters")
            .Matches("^[a-zA-Z0-9]*$").WithMessage("RoleName must not contain spaces or special characters.");
        }
    }
}
