using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.EmployeeRequests
{
    public class EmployeeFilter
    {

        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
