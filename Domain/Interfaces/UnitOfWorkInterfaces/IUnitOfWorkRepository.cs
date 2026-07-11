using Domain.Interfaces.IdentityInterfaces.RoleInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Domain.Interfaces.LoginInterfaces;
using Domain.Interfaces.LoginInterfaces.TokenInterfaces;
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
        public IRefreshTokenRepository RefreshTokenRepository { get; }
        public IRoleRepository RoleRepository { get; }

        public Task<int> CompleteAsync();
    }
}
