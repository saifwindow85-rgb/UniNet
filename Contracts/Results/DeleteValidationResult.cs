using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Results
{
    public  class DeleteValidationResult
    {
        public bool CanBeDeleted { get; set; }
        public string ?BlockingReason { get; set; }
    }
}
