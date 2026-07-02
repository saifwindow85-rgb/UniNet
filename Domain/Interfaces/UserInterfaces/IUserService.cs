using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.UserInterfaces
{
    public interface IUserService
    {
        public Task<bool> IsUserExists(string userName);
    }
}
