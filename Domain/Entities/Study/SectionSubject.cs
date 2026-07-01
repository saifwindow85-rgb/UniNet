using Domain.Entities.Academic_Structure;
using Domain.Entities.Common;
using Domain.Entities.Employees;

namespace Domain.Entities.Study
{
    public class SectionSubject : BaseEntity
    {
        public int SectionSubjectId { get; set; }

        public int SectionId { get; set; }
        public Section Section { get; set; } = null!;

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public int SemesterId { get; set; }
        public Semester Semester { get; set; } = null!;
        public string LecturerName { get; set; } = null!;
        public ICollection<StudentResult> StudentResults { get; set; } = new List<StudentResult>();
    }
}
