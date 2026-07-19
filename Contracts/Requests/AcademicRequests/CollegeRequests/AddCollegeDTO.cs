using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.AcademicRequests.CollegeRequests
{
    public  class AddCollegeDTO
    {
        public int UniversityId { get; set; }
        public string CollegeName { get; set; } = null!;
        public string ?Description { get; set; }

    }
}
