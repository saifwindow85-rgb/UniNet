using Contracts.Requests.IdentityRequests.UserRoleRequsets;
using Contracts.Responses;
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
    public interface IUserRoleService
    {
        public Task<UserRoleDTO?> FindUserRoleDTOById(int userId, int roleId);
        public Task<UserRole?>FindUserRoleEntityById(int userId, int roleId);
        public Task<PagedResult<UserRoleDTO>> GetAllUserRole(int pageNumber,int pageSize);
        public Task<PagedResult<UserRoleDTO>> GetUserRolesPerRoleId(int roleId, int pageNumber, int pageSize);
        public Task<bool> Delete(int userId, int roleId);
        public Task<AddUpdateServiceResponse<UserRoleDTO>>AddUserRole(AddUserRoleDTO addUserRoleDTO);
        public Task<bool> IsUserRoleExists(int userId, int roleId);
    }
}
