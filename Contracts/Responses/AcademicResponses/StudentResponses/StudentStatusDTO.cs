using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses.AcademicResponses.StudentResponses
{
    public class StudentStatusDTO
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; } = null!;
        public string?Description { get; set; }
    }
}
