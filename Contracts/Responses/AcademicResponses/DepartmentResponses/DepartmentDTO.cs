using Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses.AcademicResponses.DepartmentResponses
{
    public class DepartmentDTO : AddUpdateServiceAbstract
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public string?Description { get; set; } = null!;
    }
}
