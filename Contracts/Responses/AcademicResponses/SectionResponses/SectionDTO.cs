using Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses.AcademicResponses.SectionResponses
{
    public class SectionDTO : DTOsBaseEntity
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; } = null!;
        public int BatchId { get; set; }
        public string BatchName { get; set; } = null!;

    }
}
