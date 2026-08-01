using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses.StudentResponses
{
    public class StudentDTO
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string StudentNumber { get; set; } = null!;
        public int BatchId { get; set; }
        public string BatchName { get; set; } = null!;
        public int?SectionId { get; set; }
        public string?SectionName { get; set; }
        public string StatusName { get; set; } = null!;
    }
}
