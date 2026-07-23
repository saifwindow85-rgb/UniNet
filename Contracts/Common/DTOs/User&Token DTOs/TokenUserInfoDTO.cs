using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Common.DTOs.User_Token_DTOs
{
    public class TokenUserInfoDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public int? UniversityId { get; set; }
        public List<string>UserRoles { get; set; } = new List<string>();
    }
}
