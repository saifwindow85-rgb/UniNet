using Domain.Interfaces.LoginInterfaces;
using Domain.Interfaces.UserInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.UnitOfWork
{
    public interface IUnitOfWorkRepository : IDisposable
    {
        public IUserRepository UserRepository { get; }
        public ILoginRepository LoginRepository { get; }

        public Task<int> CompleteAsync();
    }
}
