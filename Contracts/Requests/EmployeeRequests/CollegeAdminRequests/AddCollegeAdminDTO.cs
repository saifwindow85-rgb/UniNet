using Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.EmployeeRequests.CollegeAdminRequests
{
    public class AddCollegeAdminDTO : BaseAddEmployeeOrStudentDTO
    {
        public int CollegeId { get; set; }
        
    }
}
