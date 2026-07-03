using Domain.Interfaces.LoginInterfaces;
using Domain.Interfaces.UnitOfWork;
using Domain.Interfaces.UserInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UnitOfWorkServices : IUnitOfWorkService
    {
        public IUserService UserService { get; private set; }

        public ILoginService LoginService { get; private set; }

        public UnitOfWorkServices()
        {

        }
    }
}
