using Contracts.Responses;
using Contracts.Responses.IdentityResponses.RoleResponses;
using Contracts.Results;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.IdentityInterfaces.RoleInterfaces
{
    public interface IRoleRepository
    {
        public Task<PagedResult<RoleDTO>> GetRoles(int pageNumber, int pageSize);
        public Task AddRole(Role role);
        public Task<Role?> GetRoleEntityById(int roleId);
        public Task<RoleDTO?> GetRoleDTOById(int roleId);
        public Task<bool> Delete(int roleId);
    }
}
