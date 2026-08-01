using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.StudentRequests
{
    public class StudentFilter
    {
        public string? FullName { get; set; }
        public string? StudentNumber { get; set; }
        public int?Status { get; set; }
        public bool? IsActive { get; set; }
        public int?BatchId { get; set; }
    }
}
