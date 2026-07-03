using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Token
{
    public class RefreshToken
    {
        public int RefreshTokenId { get; set; }

        public string TokenHash { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public int? ReplacedByTokenId { get; set; }
        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
