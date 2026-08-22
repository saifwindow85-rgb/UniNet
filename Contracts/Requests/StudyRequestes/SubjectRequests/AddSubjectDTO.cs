using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.StudyRequestes.SubjectRequests
{
    public class AddSubjectDTO
    {
        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int? CreditHours { get; set; }

        public int DepartmentId { get; set; }
    }
}
