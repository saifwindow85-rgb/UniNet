using Contracts.Requests.StudyRequestes.StudentResultRequests;
using Domain.Entities.StudyConstants;
using FluentValidation;

namespace Application.Validators.StudyValidators.StudentResultValidators
{
    public class UpdateStudentResultValidator : AbstractValidator<UpdateStudentResultDTO>
    {
        public UpdateStudentResultValidator()
        {
            RuleFor(s => s.Midterm)
                .InclusiveBetween(0, GradeConstants.MidtermMax)
                .WithMessage($"Midterm must be between 0 and {GradeConstants.MidtermMax}.");

            RuleFor(s => s.Practical)
                .InclusiveBetween(0, GradeConstants.PracticalMax)
                .WithMessage($"Practical must be between 0 and {GradeConstants.PracticalMax}.");

            RuleFor(s => s.Final)
                .InclusiveBetween(0, GradeConstants.FinalMax)
                .WithMessage($"Final must be between 0 and {GradeConstants.FinalMax}.");
        }
    }
}
