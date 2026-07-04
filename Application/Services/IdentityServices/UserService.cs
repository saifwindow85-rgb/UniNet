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
        private readonly IValidator<AddUserDTO> _validaor;
        public UserService(IUnitOfWorkRepository unitOfWork,IValidator <AddUserDTO>validator)
        {
            _unitOfWork = unitOfWork;
            _validaor = validator;
        }

        public async Task<AddUpdateServiceResponse<UserDTO>> AddUser(AddUserDTO newUser,int userId)
        {
            var validation = _validaor.Validate(newUser);
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
            return AddUpdateServiceResponse<UserDTO>.Success(userDTO);
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

        public bool VerifyPassword(string password,string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
