using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.RequestParameters
{
    public class UserRoleParameters
    {
        [Required(ErrorMessage = "UserId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid UserId Format!/Id Must Be > 0")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "RoleId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid RoleId Format!/Id Must Be > 0")]
        public int RoleId { get; set; }
    }
}
