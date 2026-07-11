using Application.Mappers.IdentityMappers;
using Contracts.Enums;
using Contracts.Requests.IdentityRequests.RoleRequests;
using Contracts.Responses;
using Contracts.Responses.IdentityResponses.RoleResponses;
using Contracts.Results;
using Domain.Entities.Identity;
using Domain.Interfaces.IdentityInterfaces.RoleInterfaces;
using Domain.Interfaces.UnitOfWork;
using FluentValidation;
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
        private readonly IValidator<AddRoleDTO> _roleValidator;

        public RoleService(IUnitOfWorkRepository unitOfWorkRepository,IValidator<AddRoleDTO>roleValidator)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            _roleValidator = roleValidator;
        }

        public async Task<AddUpdateServiceResponse<RoleDTO>> AddRole(AddRoleDTO newRole, int currentUserId)
        {
            var validationResult = await _roleValidator.ValidateAsync(newRole);
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<RoleDTO>.Failure(validationResult.Errors
                    .Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }
            if(await IsRoleExists(newRole.RoleName))
            {
                return AddUpdateServiceResponse<RoleDTO>.AlreadyExists<Role>();
            }

            var roleEntity = new Role
            {
                Name = newRole.RoleName
            };

            await _unitOfWorkRepository.RoleRepository.AddRole(roleEntity);
            await _unitOfWorkRepository.CompleteAsync();
            var roleDTO = roleEntity.ToDTO();
            return AddUpdateServiceResponse<RoleDTO>.Success(roleDTO);
        }

        public async Task<bool> Delete(int roleId)
        {
            var result = await _unitOfWorkRepository.RoleRepository.Delete(roleId);
            if(result)
            {
                await _unitOfWorkRepository.CompleteAsync();
                return result;
            }
            return result;
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

        public async Task<bool> IsRoleExists(int roleId)
        {
            return await _unitOfWorkRepository.RoleRepository.IsRoleExists(roleId);
        }

        public async Task<AddUpdateServiceResponse<RoleDTO>> UpdateRole(AddRoleDTO updatedRole, int updatedRoleId, int currentUserId)
        {
            var validationResult = await _roleValidator.ValidateAsync(updatedRole);//Notice  that i used the same DTO for add and update in Role
            var role = await FindRoleEntityById(updatedRoleId);                          //becuse there is no need for another one currently they both share same proprties
          
            if(role == null)
            {
                return AddUpdateServiceResponse<RoleDTO>.ResourceDoesntExist<Role>();
            }
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<RoleDTO>.Failure(validationResult.Errors
                  .Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

           if(await IsRoleExists(updatedRole.RoleName))
            {
                return AddUpdateServiceResponse<RoleDTO>.AlreadyExists<Role>();
            }

            role.Name = updatedRole.RoleName;

            await _unitOfWorkRepository.CompleteAsync();
            var roleDTO = role.ToDTO();
            return AddUpdateServiceResponse<RoleDTO>.Success(roleDTO);
        }
    }
}
