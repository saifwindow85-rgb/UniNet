using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.LoginRequests
{
    public class RefreshToken_LogOut_Request
    {
        public string RefreshToken { get; set; } = null!;
    }
}
