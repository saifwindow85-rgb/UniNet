using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses.IdentityResponses.UserRoleResponse
{
    public class UserRoleDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
    }
}
