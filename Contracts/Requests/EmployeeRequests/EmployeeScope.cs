using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.EmployeeRequests
{
    public class EmployeeScope
    {
        public int? UniversityId { get; set; }

        public int? CollegeId { get; set; }

        public int? DepartmentId { get; set; }
    }
}
