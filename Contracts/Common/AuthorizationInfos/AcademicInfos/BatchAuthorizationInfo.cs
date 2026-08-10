using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Common.AuthorizationInfos.AcademicInfos
{
    public class BatchAuthorizationInfo
    {
        public int UniversityId { get; set; }
        public int CollegeId { get; set; }
        public int DepartmentId { get; set; }
        public int BatchId { get; set; }
    }
}
