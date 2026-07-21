using Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses.AcademicResponses.BatchResponses
{
    public class BatchDTO : DTOsBaseEntity
    {
        public int BatchId { get; set; }
        public string BatchName { get; set; } = null!;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public int BatchYear { get; set; }
        public string?Description { get; set; }
    }
}
