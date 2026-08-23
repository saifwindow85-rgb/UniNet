using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.StudyRequestes.SubjectRequests
{
    public class SubjectFilterDTO
    {
        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int?DepartmentId { get; set; }
    }
}
