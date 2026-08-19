using Contracts.Requests.StudentRequests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.StudentValidator
{
    public class AddStudentValidator : AbstractValidator<AddStudentDTO>
    {
        public AddStudentValidator() 
        {
            // 1. Full Name
            RuleFor(x => x.FullName)
                         .NotEmpty().WithMessage("Full name is required.")
                         .MinimumLength(3).WithMessage("Full name must be at least 3 characters long.")
                         .MaximumLength(250).WithMessage("Full name cannot exceed 250 characters.")
                         .Matches(@"^[a-zA-ZÀ-ÿ\s'-]+$").WithMessage("Full name can only contain letters, spaces, hyphens, and apostrophes.");

            // 2. User Name
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(4).WithMessage("Username must be at least 4 characters long.")
                .MaximumLength(250).WithMessage("Username cannot exceed 250 characters.")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain alphanumeric characters and underscores (_), with no spaces.");

            // 3. Password (Robust Security Policy)
            RuleFor(x => x.Password)
                   .NotEmpty()
                       .WithMessage("Password is required.")
                   
                   .Length(8, 32)
                       .WithMessage("Password must be between 8 and 32 characters.")
                   
                   .Matches("[A-Z]")
                       .WithMessage("Password must contain at least one uppercase letter.")
                   
                   .Matches("[a-z]")
                       .WithMessage("Password must contain at least one lowercase letter.")
                   
                   .Matches(@"\d")
                       .WithMessage("Password must contain at least one digit.")
                   
                   .Matches(@"[^a-zA-Z0-9]")
           .WithMessage("Password must contain at least one special character.");

            // 4. Email (Optional, but validated if provided)
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email address format.")
                .MaximumLength(150).WithMessage("Email cannot exceed 150 characters.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            // 5. Phone Number (Optional, E.164 international standard format)
            RuleFor(x => x.PhoneNumber).MinimumLength(9).WithMessage("Phone number must be at least 9 digits long.")
             .MaximumLength(25).WithMessage("Phone number cannot exceed 25 digits.")
             .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format. Please use international format (e.g., +1234567890).")
             .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            // 6. Student Number
            RuleFor(x => x.StudentNumber)
                .NotEmpty().WithMessage("Student number is required.")
                .MinimumLength(9).WithMessage("Student number must be at least 9 digits long!")
                .MaximumLength(20).WithMessage("Student number cannot exceed 25 digits!")
                  .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid student number format!")
                  .When(x => !string.IsNullOrEmpty(x.StudentNumber));

        }
    }
}
