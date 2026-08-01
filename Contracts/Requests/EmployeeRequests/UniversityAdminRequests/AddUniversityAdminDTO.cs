using Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.EmployeeRequests.UniversityAdminRequests
{
    public class AddUniversityAdminDTO : BaseAddEmployeeOrStudentDTO
    {
        public int UniversityId { get; set; } // only required if the user is a UniversityAdmin
        
    }
}
