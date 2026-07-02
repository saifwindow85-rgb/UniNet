using Contracts.DTOs.Login_Request_DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Validators.LoginRequestValidator
{
    public class LoginRequestValidator : AbstractValidator<LoginRequestDTO>
    {
        public LoginRequestValidator() 
        {
            RuleFor(l => l.UserName).NotEmpty().MinimumLength(4).WithMessage("Minimum length is 4 letters!")
                .MaximumLength(250).WithMessage("Max lenght is 250 letters!")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("UserName can only contain letters, numbers, and underscores.");

            RuleFor(l => l.Password).MinimumLength(9).WithMessage("Minimum lenght is 9 letters")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("Password can only contain letters, numbers, and underscores.");
        }
    }
}
