using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.LoginRequests
{
    public class UserToken
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public int? UniversityId { get; set; }
        public List<string>UserRoles { get; set; } = new List<string>();
        public string TokenHash { get; set; } = null!;
        public int RefreshTokenId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public int? ReplacedByTokenId { get; set; }
    }
}
