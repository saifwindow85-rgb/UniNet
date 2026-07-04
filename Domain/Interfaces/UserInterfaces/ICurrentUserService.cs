using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.UserInterfaces
{
    public interface ICurrentUserService
    {
        public int UserId { get; }
        public string UserName { get; }
    }
}
