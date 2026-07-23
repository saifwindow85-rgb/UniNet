using Domain.Entities.Common;
using Domain.Entities.Employees;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Academic_Structure
{
    public class University :BaseEntity
    {
        public int UniversityId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }
        public ICollection<College> Colleges { get; set; }
          = new List<College>();
        public ICollection <User> Users { get; set; } = new List<User>();
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();

    }
}
