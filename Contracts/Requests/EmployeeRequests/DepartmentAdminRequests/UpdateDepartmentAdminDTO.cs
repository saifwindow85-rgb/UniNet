using Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.EmployeeRequests.DepartmentAdminRequests
{
    public class UpdateDepartmentAdminDTO : BaseUpdateEmployeeDTO
    {
        public int DepartmentId { get; set; }
    }
}
