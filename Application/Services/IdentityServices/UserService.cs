using Domain.Interfaces.UnitOfWork;
using Domain.Interfaces.UserInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Contracts.Responses;
using Contracts.Requests.UserRequests;
using FluentValidation;
using Application.Validators;
using Contracts.Enums;
using Domain.Entities.Identity;

namespace Application.Services.IdentityServices
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWorkRepository _unitOfWork;
        private readonly IValidator<AddUserDTO> _addUserValidaor;
        private readonly IValidator<UpdateUserDTO> _updateUserValidator;
        public UserService(IUnitOfWorkRepository unitOfWork,IValidator <AddUserDTO>addUserValidator,IValidator<UpdateUserDTO> updateUserValidator)
        {
            _unitOfWork = unitOfWork;
            _addUserValidaor = addUserValidator;
            _updateUserValidator = updateUserValidator;
        }

        public async Task<AddUpdateServiceResponse<UserDTO>> AddUser(AddUserDTO newUser,int userId)
        {
            var validation = _addUserValidaor.Validate(newUser);
            if(!validation.IsValid)
            {
                return AddUpdateServiceResponse<UserDTO>.Failure(validation.Errors.Select
                    (e => $"{e.PropertyName} : {e.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            if(await _unitOfWork.UserRepository.IsUserExsist(newUser.UserName))
            {
                return AddUpdateServiceResponse<UserDTO>.ExistedResource<User>();
            }

            var userEntity = new User
            {
                FullName = newUser.FullName,
                UserName = newUser.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.Password),
                PhoneNumber = newUser.PhoneNumber,
                Email = newUser.Email,
                IsActive = newUser.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
            };
            await _unitOfWork.UserRepository.Add(userEntity);
            await _unitOfWork.CompleteAsync();
            var userDTO = await _unitOfWork.UserRepository.GetUserById(userEntity.UserId);
            return AddUpdateServiceResponse<UserDTO>.Success(userDTO!);
        }

        public async Task<UserDTO?> FindById(int Id)
        {
            return await _unitOfWork.UserRepository.GetUserById(Id);
        }

        public async Task<User?> FindByUserName(string userName)
        {
            return await _unitOfWork.UserRepository.GetUserByUserName(userName);
        }

        public async Task<bool> IsUserExists(string userName)
        {
            return await _unitOfWork.UserRepository.IsUserExsist(userName);
        }

        public async Task<AddUpdateServiceResponse<UserDTO>> UpdateUser(int updatedUserId, UpdateUserDTO updatedUser, int userId)
        {
            var user = await _unitOfWork.UserRepository.GetUserEntityById(updatedUserId);
            if(user == null)
            {
                return AddUpdateServiceResponse<UserDTO>.ResourceDoesntExsist<User>();
            }
            var validationResult = _updateUserValidator.Validate(updatedUser);
            if (!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<UserDTO>
                    .Failure(validationResult.Errors.Select(e => $"{e.PropertyName} : {e.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }
            if(await IsUserExists(updatedUser.UserName) && user.UserName != updatedUser.UserName)
            {
                return AddUpdateServiceResponse<UserDTO>.ExistedResource<User>(updatedUser.UserName);
            }
            user.FullName = updatedUser.FullName;
            user.UserName = updatedUser.UserName;
            user.Email = updatedUser.Email;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedByUserId = userId;
            user.IsActive = updatedUser.IsActive;
            var userDTO = await FindById(user.UserId);
            await _unitOfWork.CompleteAsync();
            return AddUpdateServiceResponse<UserDTO>.Success(userDTO!);
        }

        public bool VerifyPassword(string password,string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

    }
}
