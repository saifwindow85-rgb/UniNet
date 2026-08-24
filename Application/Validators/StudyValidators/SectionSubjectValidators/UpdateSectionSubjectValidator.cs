using Contracts.Requests.StudyRequestes.SectionSubjectRequests;
using FluentValidation;

namespace Application.Validators.StudyValidators.SectionSubjectValidators
{
    public class UpdateSectionSubjectValidator : AbstractValidator<UpdateSectionSubjectDTO>
    {
        public UpdateSectionSubjectValidator()
        {
            RuleFor(s => s.LecturerName)
                .NotEmpty().WithMessage("Lecturer name is required.")
                .MinimumLength(2).WithMessage("Lecturer name must be at least 2 characters long.")
                .MaximumLength(200).WithMessage("Lecturer name must not exceed 200 characters.");
        }
    }
}
