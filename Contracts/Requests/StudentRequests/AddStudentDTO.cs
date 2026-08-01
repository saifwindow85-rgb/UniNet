using Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.StudentRequests
{
    public class AddStudentDTO :BaseAddEmployeeOrStudentDTO
    {
        public string StudentNumber { get; set; } = null!;
        public int BatchId { get; set; }
        public int?SectionId { get; set; }
        public int StatusId { get; set; }
    }
}
