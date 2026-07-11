using Contracts.Requests.IdentityRequests.RoleRequests;
using Contracts.Responses;
using Contracts.Responses.IdentityResponses.RoleResponses;
using Contracts.Results;
using Domain.Entities.Identity;
using Domain.Interfaces.IdentityInterfaces.RoleInterfaces;
using Domain.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.IdentityServices
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        public RoleService(IUnitOfWorkRepository unitOfWorkRepository)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public Task<AddUpdateServiceResponse<RoleDTO>> AddRole(AddRoleDTO newRole, int currentUserId)
        {
            throw new NotImplementedException();
        }

        public async Task<RoleDTO?> FindRoleDTOById(int roleId)
        {
            return await _unitOfWorkRepository.RoleRepository.GetRoleDTOById(roleId);
        }

        public async Task<RoleDTO?> FindRoleDTOByRoleName(string roleName)
        {
            return await _unitOfWorkRepository.RoleRepository.GetRoleDTOByRoleName(roleName);
        }

        public Task<Role?> FindRoleEntityById(int roleId)
        {
            return _unitOfWorkRepository.RoleRepository.GetRoleEntityById(roleId);
        }

        public async Task<PagedResult<RoleDTO>> GetRoles(int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.RoleRepository.GetRoles(pageNumber, pageSize);
        }

        public async Task<bool> IsRoleExists(string roleName)
        {
            return await _unitOfWorkRepository.RoleRepository.IsRoleExists(roleName);
        }

        public Task<AddUpdateServiceResponse<RoleDTO>> UpdateRole(AddRoleDTO updatedRole, int updatedRoleId, int currentUserId)
        {
            throw new NotImplementedException();
        }
    }
}
