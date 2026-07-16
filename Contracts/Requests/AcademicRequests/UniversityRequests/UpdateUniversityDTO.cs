using Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.AcademicRequests.UniversityRequests
{
    public class UpdateUniversityDTO 
    {
        public string UniversityName { get; set; } = null!;
        public string? Description { get; set; }
    }
}
