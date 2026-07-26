using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses.EmployeeResponse
{
    public class EmployeeDTO
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = null!;
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public bool IsActive { get; set; }
        public string ?Email { get; set; }
        public string ?PhoneNumber { get; set; }
        public int UniversityId { get; set; }
        public string UniversityName { get; set; } = null!;
        public int? CollegeId { get; set; }
        public string? CollegeName { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
    }
}
