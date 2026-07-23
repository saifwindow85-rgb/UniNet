using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses.IdentityResponses
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;

        public string UserName { get; set; } = null!;
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt {  get; set; }
        public int? CreatedByUserId { get; set; }
        public string ?CreatorUserName { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? UpdatedByUserId { get; set; }
        public string? UpdaterUserName { get; set; }
        public int? UniversityId { get; set; } // only required if the user is a UniversityAdmin
        public string? UniversityName { get; set; } // only required if the user is a UniversityAdmin
    }
}
