using Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses.CollegeResponses
{
    public class CollegeDTO : AddUpdateServiceAbstract
    {
        public int CollegeId { get; set; }
        public string CollegeName { get; set; } = null!;
        public int UniversityId { get; set; }
        public string UniversityName { get; set; } = null!;
        public string ?Description {  get; set; } 
    }
}
