using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Common
{
    public abstract class UpdateServiceAbstract
    {
        public int UpdatedByUserId { get; set; } // It can not be nullable as entity because whene you update it must not be null
    }
}
