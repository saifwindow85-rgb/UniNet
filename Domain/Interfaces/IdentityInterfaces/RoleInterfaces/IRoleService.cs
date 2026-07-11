using Contracts.Requests.IdentityRequests.RoleRequests;
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
    public interface IRoleService
    {
        public Task<PagedResult<RoleDTO>> GetRoles(int pageNumber, int pageSize);
        public Task<RoleDTO?> FindRoleDTOById(int roleId);
        public Task<Role?> FindRoleEntityById(int roleId);
        public Task<RoleDTO?> FindRoleDTOByRoleName(string roleName);
        public Task<AddUpdateServiceResponse<RoleDTO>> AddRole(AddRoleDTO newRole);
        public Task<AddUpdateServiceResponse<RoleDTO>>UpdateRole(AddRoleDTO updatedRole,int updatedRoleId);
        public Task<bool> IsRoleExists(string roleName);
        public Task<bool> IsRoleExists(int roleId);
        public Task<bool> Delete(int roleId);
    }
}
