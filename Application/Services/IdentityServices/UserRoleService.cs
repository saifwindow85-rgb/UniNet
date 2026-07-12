using Contracts.Requests.IdentityRequests.UserRoleRequsets;
using Contracts.Responses;
using Contracts.Responses.IdentityResponses.UserRoleResponse;
using Contracts.Results;
using Domain.Entities.Identity;
using Domain.Interfaces.IdentityInterfaces.UserRoleInterfaces;
using Domain.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.IdentityServices
{
    internal class UserRoleService : IUserRoleService
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        public UserRoleService(IUnitOfWorkRepository unitOfWorkRepository)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<AddUpdateServiceResponse<UserRoleDTO>> AddUserRole(AddUserRoleDTO addUserRoleDTO)
        {
            if(! await _unitOfWorkRepository.UserRepository.IsUserExists(addUserRoleDTO.UserId)
                &&! await _unitOfWorkRepository.RoleRepository.IsRoleExists(addUserRoleDTO.RoleId))
            {
                return AddUpdateServiceResponse<UserRoleDTO>.InvalidRelatedData();
            }

            if(await IsUserRoleExists(addUserRoleDTO.UserId,addUserRoleDTO.RoleId))
            {
                return AddUpdateServiceResponse<UserRoleDTO>.AlreadyExists<UserRole>();
            }

            var userRoleEntity = new UserRole
            {
                UserId = addUserRoleDTO.UserId,
                RoleId = addUserRoleDTO.RoleId,
            };
            await _unitOfWorkRepository.UserRoleRepository.Add(userRoleEntity);
            await _unitOfWorkRepository.CompleteAsync();
            var userRoleDTO = await FindUserRoleDTOById(userRoleEntity.UserId, userRoleEntity.RoleId);
            return AddUpdateServiceResponse<UserRoleDTO>.Success(userRoleDTO!);
        }

        public async Task<bool> Delete(int userId, int roleId)
        {
            var result = await _unitOfWorkRepository.UserRoleRepository.Delete(userId, roleId);
            if(result)
            {
               await _unitOfWorkRepository.CompleteAsync();
                return result;
            }
            return result;
        }

        public async Task<UserRoleDTO?> FindUserRoleDTOById(int userId, int roleId)
        {
            return await _unitOfWorkRepository.UserRoleRepository.GetUserRoleDTOById(userId, roleId);
        }

        public async Task<UserRole?> FindUserRoleEntityById(int userId, int roleId)
        {
            return await _unitOfWorkRepository.UserRoleRepository.GetUserRoleEntityById(userId, roleId);
        }

        public async Task<PagedResult<UserRoleDTO>> GetAllUserRoleDTOs(int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.UserRoleRepository.GetUserRoles(pageNumber, pageSize);
        }

        public async Task<PagedResult<UserRoleDTO>> GetUserRolesPerRoleId(int roleId, int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.UserRoleRepository.GetUsersPerRoleId(roleId, pageNumber, pageSize);
        }

        public async Task<bool> IsUserRoleExists(int userId, int roleId)
        {
            return await _unitOfWorkRepository.UserRoleRepository.IsUserRoleExists(userId, roleId);
        }

        public async Task<AddUpdateServiceResponse<UserRoleDTO>> UpdateUserRole(UpdateUserRoleDTO updateUserRoleDTO, int userId, int roleId)
        {
            var userRole = await FindUserRoleEntityById(userId, roleId);
            if(userRole == null)
            {
                return AddUpdateServiceResponse<UserRoleDTO>.ResourceDoesntExist<UserRole>();
            }

            userRole.RoleId = updateUserRoleDTO.RoleId;
            await _unitOfWorkRepository.CompleteAsync();
            var userRoleDTO = await FindUserRoleDTOById(userRole.UserId, userRole.RoleId);
            return AddUpdateServiceResponse<UserRoleDTO>.Success(userRoleDTO!);
        }
    }
}
