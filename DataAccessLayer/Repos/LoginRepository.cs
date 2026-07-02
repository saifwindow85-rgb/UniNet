using DataAccessLayer.Dbcontext;
using Domain.Interfaces.LoginInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos
{
    public class LoginRepository : ILoginRepository
    {
        private readonly AppDbcontext _context;

        public LoginRepository(AppDbcontext context)
        {
            _context = context;
        }
        public async Task<bool> IsUserExists(string userName)
        {
            return await _context.Users.AnyAsync(l => l.UserName == userName);
        }
    }
}
