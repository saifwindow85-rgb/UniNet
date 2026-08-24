using Contracts.Requests.StudyRequestes.SemesterRequests;
using FluentValidation;

namespace Application.Validators.StudyValidators.SemesterValidators
{
    public class AddSemesterValidator : AbstractValidator<AddSemesterDTO>
    {
        public AddSemesterValidator()
        {
            RuleFor(s => s.Name)
                .NotEmpty().WithMessage("Semester name is required.")
                .MinimumLength(3).WithMessage("Semester name must be at least 3 characters long.")
                .MaximumLength(200).WithMessage("Semester name must not exceed 200 characters.");

            RuleFor(s => s.UniversityId)
                .GreaterThan(0).WithMessage("A valid UniversityId is required.");

            RuleFor(s => s.StartDate)
                .NotEmpty().WithMessage("Start date is required.");

            RuleFor(s => s.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .GreaterThan(s => s.StartDate).WithMessage("End date must be after the start date.");
        }
    }
}
