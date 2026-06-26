using Domain.Entities.Content;
using Domain.Entities.Employees;
using Domain.Entities.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Identity
{
    public class User
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }


        public ICollection<UserRole> UserRoles { get; set; }
            = new List<UserRole>();
        public ICollection<Post> Posts { get; set; }
    = new List<Post>();

        public ICollection<Announcement> Announcements { get; set; }
            = new List<Announcement>();

    }
}
