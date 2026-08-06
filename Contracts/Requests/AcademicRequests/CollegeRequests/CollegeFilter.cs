using Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.AcademicRequests.CollegeRequests
{
    public class CollegeFilter : BaseFilterCoulmns
    {
        public int? UniversityId { get; set; }
    }
}
