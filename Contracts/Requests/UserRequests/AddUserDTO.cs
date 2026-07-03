using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.UserRequests
{
    public class AddUserDTO
    {

        public string FullName { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }

    }
}
