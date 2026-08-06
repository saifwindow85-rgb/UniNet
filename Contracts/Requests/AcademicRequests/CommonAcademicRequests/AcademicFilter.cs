using Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.AcademicRequests.CommonAcademicRequests
{
    public class AcademicFilter : BaseFilterCoulmns
    {
        public int? UniversityId { get; set; }
        public int?CollegeId { get; set; }
        public int?DepartmentId { get; set; }
        public int?BatchId { get; set; }
        public int?SectionId { get; set; }
    }
}
