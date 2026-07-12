using Contracts.Responses.IdentityResponses.UserRoleResponse;
using Contracts.Results;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.IdentityInterfaces.UserRoleInterfaces
{
    public interface IUserRoleRepository
    {
        public Task Add(UserRole userRole);
        public Task<PagedResult<UserRoleDTO>> GetUserRoles(int pageNumber, int pageSize);
        public Task<PagedResult<UserRoleDTO>> GetUsersPerRoleId(int roleId, int pageNumber, int pageSize);
        public Task<UserRoleDTO?> GetUserRoleDTOById(int userId, int roleId);
        public Task<UserRole?>GetUserRoleEntityById(int userId, int roleId);
        public Task<bool> Delete(int userId, int roleId);
        public Task<bool> IsUserRoleExists(int userId, int roleId);
    }
}
