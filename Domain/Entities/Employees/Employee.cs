using Domain.Entities.Academic_Structure;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Employees
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int UniversityId { get; set; }

        public int? CollegeId { get; set; }

        public int? DepartmentId { get; set; }

        public University University { get; set; } = null!;

        public College? College { get; set; }

        public Department? Department { get; set; }
    }
}
