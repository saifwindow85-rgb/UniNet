using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses.Login
{
    public class TokenResponse
    {
        public string AccesseToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
