using Domain.Entities.Common;
using Domain.Entities.Identity;
using Domain.Entities.Students;
using Domain.Entities.StudyConstants;

namespace Domain.Entities.Study
{
    public class StudentResult : BaseEntity
    {
        public int StudentResultId { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int SectionSubjectId { get; set; }
        public SectionSubject SectionSubject { get; set; } = null!;
        public decimal Midterm { get; set; }

        public decimal Practical { get; set; }

        public decimal Final { get; set; }

        public decimal Total { get; private set; }

        public StudentResult SetGrades(decimal midTerm,decimal practical,decimal final)
        {
            if (midTerm < 0 || midTerm > GradeConstants.MidtermMax)
                throw new ArgumentOutOfRangeException(nameof(midTerm));

            if(practical < 0 || practical > GradeConstants.PracticalMax)
                throw new ArgumentException(nameof(practical));

            if(final < 0 || final > GradeConstants.FinalMax)
                throw new ArgumentException(nameof(final));

            Midterm = midTerm;
            Practical = practical;
            Final = final;

            // Total has a computed sql column 
            return this;
        }
    }
}
