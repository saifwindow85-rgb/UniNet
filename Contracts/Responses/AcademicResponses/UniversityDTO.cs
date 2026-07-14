using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses.AcademicResponses
{
    public  class UniversityDTO
    {
        public int UniversityId { get; set; }
        public string UniversityName { get; set; } = null!;
        public string ?Description { get; set; } = null!;
    }
}
