using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.AcademicRequests.DepartmentRequests
{
    public class UpdateDepartmentDTO
    {
        public string DepartmentName { get; set; } = null!;
        public string?Description { get; set; }
    }
}
