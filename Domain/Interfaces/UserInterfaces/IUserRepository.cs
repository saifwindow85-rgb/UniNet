using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.UserInterfaces
{
    public interface IUserRepository
    {
        public  Task<bool> IsUserExsist(string userName);
    }
}
