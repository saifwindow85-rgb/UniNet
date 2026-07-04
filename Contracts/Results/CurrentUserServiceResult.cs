using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Results
{
    public class CurrentUserServiceResult
    {
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public bool IsSuccess { get; set; }
    }
}
