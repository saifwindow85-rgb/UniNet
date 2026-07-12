using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.IdentityRequests.UserRoleRequsets
{
    public class AddUserRoleDTO
    {
        // There is no fluent validation for this DTO 
        // Becuse IdParameter class is enough for this situation
        public int UserId { get; set; } 
        public int RoleId { get; set; }
    }
}
