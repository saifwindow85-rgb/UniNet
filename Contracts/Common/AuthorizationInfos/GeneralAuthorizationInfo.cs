using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Common.AuthorizationInfos
{
    public class GeneralAuthorizationInfo
    {
        public int?UniversityId { get; set; }
        public int?CollegeId { get; set; }
        public int?DepartmentId { get; set; }
        public int?BatchId { get; set; }
        public int?SectionId { get; set; }
    }
}
