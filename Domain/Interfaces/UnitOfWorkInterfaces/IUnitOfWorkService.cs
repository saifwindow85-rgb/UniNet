using Domain.Interfaces.LoginInterfaces;
using Domain.Interfaces.UserInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.UnitOfWork
{
    public interface IUnitOfWorkService
    {
        public IUserService UserService { get; }
        public ILoginService LoginService { get; }
    }
}
