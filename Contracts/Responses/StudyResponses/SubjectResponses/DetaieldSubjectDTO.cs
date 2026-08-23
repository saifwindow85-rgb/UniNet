using Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses.StudyResponses.SubjectResponses
{
    public class DetaieldSubjectDTO : DTOsBaseEntity
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string?Description { get; set; } = null!;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public int? CreditHours { get; set; }

    }
}
