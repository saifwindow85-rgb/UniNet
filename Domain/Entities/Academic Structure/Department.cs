using Domain.Entities.Common;
using Domain.Entities.Employees;
using Domain.Entities.Study;

namespace Domain.Entities.Academic_Structure
{
    public class Department : BaseEntity
    {
        public int DepartmentId { get; set; }

        public int CollegeId { get; set; }
        public College College { get; set; } = null!;
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
        public ICollection<Batch> Batches { get; set; } =new List<Batch>();

        public ICollection<Subject> Subjects { get; set; }
            = new List<Subject>();
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();

    }
}
