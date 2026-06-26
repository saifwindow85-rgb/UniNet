using Domain.Entities.Common;
using Domain.Entities.Employees;

namespace Domain.Entities.Academic_Structure
{
    public class College : BaseEntity
    {
        public int CollegeId { get; set; }

        public int UniversityId { get; set; }
        public University University { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }
        public ICollection<Department> Departments { get; set; } = new List<Department>();
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
