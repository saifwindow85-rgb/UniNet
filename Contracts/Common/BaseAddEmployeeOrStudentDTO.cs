using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Common
{
    public abstract class BaseAddEmployeeOrStudentDTO
    {
        public string FullName { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }
    }
    public abstract class BaseUpdateEmployeeOrStudentDTO
    {
        public string FullName { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }
    }
}
