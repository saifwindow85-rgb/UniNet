using Contracts.Responses.IdentityResponses;
using DataAccessLayer.Dbcontext;
using Domain.Entities.Identity;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
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
            UniversityId = u.UniversityId,
            UniversityName = u.University == null ? null : u.University.Name,
            FullName = u.FullName,
            UserName = u.UserName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            CreatedByUserId = u.CreatedByUserId,
            UpdatedAt = u.UpdatedAt,
            UpdatedByUserId = u.UpdatedByUserId,
            CreatorUserName = u.CreatedByUser == null? null : u.CreatedByUser.UserName,
            UpdaterUserName = u.UpdatedByUser == null? null : u.UpdatedByUser.UserName
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

        public async Task<User?> GetUserByUserName(string userName)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<User?> GetUserEntityById(int Id)
        {
            return await _context.Users.FindAsync(Id);
        }

        public async Task<bool> IsUserExists(int userId)
        {
            return await _context.Users.AnyAsync(u=>u.UserId == userId);
        }

        public async Task<List<string>> GetRolesNamesByUserId(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Join(_context.Roles, ur => ur.RoleId, r => r.RoleId, (ur, r) => r.Name)
                .ToListAsync();
        }
    }
}
