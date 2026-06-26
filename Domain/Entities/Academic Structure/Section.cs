using Domain.Entities.Common;
using Domain.Entities.Students;
using Domain.Entities.Study;

namespace Domain.Entities.Academic_Structure
{
    public class Section : BaseEntity
    {
        public int SectionId { get; set; }

        public int BatchId { get; set; }
        public Batch Batch { get; set; } = null!;

        public string Name { get; set; } = null!;

        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<SectionSubject> SectionSubjects { get; set; } = new List<SectionSubject>();
    }
}
