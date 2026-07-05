using Contracts.Requests.UserRequests;
using Contracts.Responses;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.UserInterfaces
{
    public interface IUserService
    {
        public Task<bool> IsUserExists(string userName);
        public Task<UserDTO?> FindById(int Id);
        public Task<User?> FindByUserName(string userName);
        public Task<AddUpdateServiceResponse<UserDTO>> AddUser(AddUserDTO newUser,int userId);
        public Task<AddUpdateServiceResponse<UserDTO>> UpdateUser(int updatedUserId,UpdateUserDTO updatedUser, int userId);
        public bool VerifyPassword(string password, string passwordHash);
    }
}
