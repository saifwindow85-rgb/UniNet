using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.StudentRequests
{
    public class AddUpdateStudentStatusDTO
    {
        public string Name { get; set; } = null!;
        public string?Description { get; set; }
    }
}
