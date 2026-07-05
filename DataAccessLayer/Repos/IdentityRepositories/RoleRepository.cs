using Contracts.Responses.IdentityResponses.RoleResponses;
using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Extensions;
using Domain.Entities.Identity;
using Domain.Interfaces.IdentityInterfaces.RoleInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos.IdentityRepositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbcontext _context;
        private readonly Expression<Func<Role, RoleDTO>> ToDTO = r => new RoleDTO
        {
            RoleId = r.RoleId,
            RoleName = r.Name,
        };
        public RoleRepository(AppDbcontext context)
        {
            _context = context;
        }

        public async Task AddRole(Role role)
        {
            await _context.Roles.AddAsync(role);
        }

        public async Task<bool> Delete(int roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
                return false;
            _context.Roles.Remove(role);
            return true;
        }

        public async Task<RoleDTO?> GetRoleDTOById(int roleId)
        {
            return await _context.Roles.AsNoTracking().Select(ToDTO).SingleOrDefaultAsync(r => r.RoleId == roleId);
        }

        public async Task<Role?> GetRoleEntityById(int roleId)
        {
            return await _context.Roles.FindAsync(roleId);
        }

        public async Task<PagedResult<RoleDTO>> GetRoles(int pageNumber, int pageSize)
        {
            return await _context.Roles.AsNoTracking().Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }
    }
}
