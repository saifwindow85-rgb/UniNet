using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.AcademicRequests.SectionRequests
{
    public class AddSectionDTO
    {
        public int BatchId { get; set; }
        public string SectionName { get; set; } = null!;
    }
}
