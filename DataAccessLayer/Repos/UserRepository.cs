using DataAccessLayer.Dbcontext;
using Domain.Interfaces.UserInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos
{
    public class UserRepository : IUserRepository
    {

        public readonly AppDbcontext _context;
        public UserRepository(AppDbcontext context)
        {
            _context = context;
        }
        public async Task<bool> IsUserExsist(string userName)
        {
            return await _context.Users.AnyAsync(u=>u.UserName == userName);
        }
    }
}
