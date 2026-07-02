using DataAccessLayer.Dbcontext;
using Domain.Interfaces.LoginInterfaces;
using Domain.Interfaces.UnitOfWork;
using Domain.Interfaces.UserInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos
{
    public class UnitOfWorkRepository : IUnitOfWorkRepository
    {
        private readonly AppDbcontext _context;
        public IUserRepository UserRepository { get; private set; }

        public ILoginRepository LoginRepository { get; private set; }

        public UnitOfWorkRepository(AppDbcontext context)
        {
            _context = context;
            UserRepository = new UserRepository(context);
            LoginRepository = new LoginRepository(context);
        }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
