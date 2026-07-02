using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.LoginInterfaces
{
    public interface ILoginRepository
    {
        public Task<bool> IsUserExists(string UserName);
    }
}
