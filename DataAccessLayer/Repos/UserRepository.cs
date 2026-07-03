using Contracts.Responses;
using DataAccessLayer.Dbcontext;
using Domain.Entities.Identity;
using Domain.Interfaces.UserInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos
{
    public class UserRepository : IUserRepository
    {
        private readonly Expression<Func<User, UserDTO>> ToDTO = u => new UserDTO
        {
            UserId = u.UserId,
            FullName = u.FullName,
            UserName = u.UserName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            IsActive = u.IsActive,
        };

        public readonly AppDbcontext _context;
        public UserRepository(AppDbcontext context)
        {
            _context = context;
        }

        public async Task<UserDTO?> GetUserById(int Id)
        {
            return  await _context.Users.Select(ToDTO).SingleOrDefaultAsync(u => u.UserId == Id);
        }

        public async Task<bool> IsUserExsist(string userName)
        {
            return await _context.Users.AnyAsync(u=>u.UserName == userName);
        }

        public async Task Add(User user)
        {
            await _context.Users.AddAsync(user);
        }
    }
}
