using Domain.Entities.Common;
using Domain.Entities.Identity;
using Domain.Entities.Students;

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

    }
}
