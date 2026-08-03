using Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.StudentRequests
{
    public class UpdateStudentDTO : BaseUpdateEmployeeOrStudentDTO
    {
        public int? SectionId { get; set; }
        public int StatusId { get; set; }
    }
}
