using Contracts.Responses.IdentityResponses;
using Contracts.Responses.IdentityResponses.UserRoleResponse;
using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Extensions;
using Domain.Entities.Identity;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserRoleInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos.IdentityRepositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly AppDbcontext _context;
        private readonly Expression<Func<UserRole, UserRoleDTO>> ToDTO = ur => new UserRoleDTO
        {
            UserId = ur.UserId,
            UserName = ur.User.UserName,
            RoleId = ur.RoleId,
            RoleName = ur.Role.Name,
        };
        public UserRoleRepository(AppDbcontext context)
        {
            _context = context;
        }

        public async Task Add(UserRole userRole)
        {
            await _context.AddAsync(userRole);
        }

        public async Task<bool> Delete(int userId, int roleId)
        {
            var userRole = await _context.UserRoles.FindAsync(userId, roleId);
            if(userRole == null)
            {
                return false;
            }
            _context.UserRoles.Remove(userRole);
            return true;
        }

        public async Task<UserRoleDTO?> GetUserRoleDTOById(int userId, int roleId)
        {
            return await _context.UserRoles.Select(ToDTO).SingleOrDefaultAsync(ur=>ur.UserId ==  userId && ur.RoleId == roleId);
        }

        public async Task<UserRole?> GetUserRoleEntityById(int userId, int roleId)
        {
           return await _context.UserRoles.FindAsync(userId,roleId);
        }

        public async Task<PagedResult<UserRoleDTO>> GetUserRoles(int pageNumber, int pageSize)
        {
           return await _context.UserRoles.AsNoTracking().OrderBy(ur=>ur.UserId).ThenBy(ur=>ur.RoleId)
                .Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<PagedResult<UserRoleDTO>> GetUsersPerRoleId(int roleId, int pageNumber, int pageSize)
        {
            return await _context.UserRoles.AsNoTracking().Where(ur=>ur.RoleId == roleId).OrderBy(ur=>ur.UserId)
                .Select(ToDTO).ToPagedResultAsync(pageNumber,pageSize);
        }

        public async Task<bool> IsUserRoleExists(int userId, int roleId)
        {
            return await _context.UserRoles.AnyAsync(ur=>ur.UserId == userId && ur.RoleId == roleId);
        }
    }
}
