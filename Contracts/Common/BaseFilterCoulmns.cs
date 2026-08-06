using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Common
{
    public class BaseFilterCoulmns
    {
        public string? Search { get; set; }
        public DateTime?StartDate { get; set; }
        public DateTime?EndDate { get; set; }

    }
}
