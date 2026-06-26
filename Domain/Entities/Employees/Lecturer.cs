using Domain.Entities.Common;
using Domain.Entities.Study;

namespace Domain.Entities.Employees
{
    public class Lecturer : BaseEntity
    {
        public int LecturerId { get; set; }

        public string FullName { get; set; } = null!;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }
        public ICollection<SectionSubject> SectionSubjects { get; set; } = new List<SectionSubject>();

    }
}
