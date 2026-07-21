using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.AcademicRequests.BatchRequests
{
    public class AddBatchDTO
    {
        public int DepartmentId { get; set; }
        public string BatchName { get; set; } = null!;
        public int BatchYear { get; set; }
        public string?Description { get; set; }
    }
}
