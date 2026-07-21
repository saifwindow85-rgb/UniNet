using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.AcademicRequests.BatchRequests
{
    public class UpdateBatchDTO
    {
        public string BatchName { get; set; } = null!;
        public int BatchYear { get; set; }
        public string? Description { get; set; }
    }
}
