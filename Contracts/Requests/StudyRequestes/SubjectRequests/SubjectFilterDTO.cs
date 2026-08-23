using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.StudyRequestes.SubjectRequests
{
    public class SubjectFilterDTO
    {
        public string? Code { get; set; }

        public string? Name { get; set; }

        public int? DepartmentId { get; set; }
    }
}
