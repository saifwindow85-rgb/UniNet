using Domain.Entities.Academic_Structure;
using Domain.Entities.Common;

namespace Domain.Entities.Study
{
    public class Subject : BaseEntity
    {
        public int SubjectId { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int CreditHours { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
        public ICollection<SectionSubject> SectionSubjects { get; set; } = new List<SectionSubject>();
    }
}
