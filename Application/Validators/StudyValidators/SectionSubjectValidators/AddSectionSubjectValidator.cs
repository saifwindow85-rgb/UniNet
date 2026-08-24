using Contracts.Requests.StudyRequestes.SectionSubjectRequests;
using FluentValidation;

namespace Application.Validators.StudyValidators.SectionSubjectValidators
{
    public class AddSectionSubjectValidator : AbstractValidator<AddSectionSubjectDTO>
    {
        public AddSectionSubjectValidator()
        {
            RuleFor(s => s.SectionId)
                .GreaterThan(0).WithMessage("A valid SectionId is required.");

            RuleFor(s => s.SubjectId)
                .GreaterThan(0).WithMessage("A valid SubjectId is required.");

            RuleFor(s => s.SemesterId)
                .GreaterThan(0).WithMessage("A valid SemesterId is required.");

            RuleFor(s => s.LecturerName)
                .NotEmpty().WithMessage("Lecturer name is required.")
                .MinimumLength(2).WithMessage("Lecturer name must be at least 2 characters long.")
                .MaximumLength(200).WithMessage("Lecturer name must not exceed 200 characters.");
        }
    }
}
