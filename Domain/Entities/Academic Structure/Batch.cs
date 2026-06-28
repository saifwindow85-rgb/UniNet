using Domain.Entities.Common;
using Domain.Entities.Students;

namespace Domain.Entities.Academic_Structure
{
    public class Batch : BaseEntity
    {
        public int BatchId { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int BatchYear { get; set; }

        public string? Description { get; set; }
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Section> Sections { get; set; } = new List<Section>();
    }
}
