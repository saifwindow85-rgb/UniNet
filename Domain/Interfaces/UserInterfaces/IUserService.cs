using Contracts.Requests.UserRequests;
using Contracts.Responses;
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
        public Task<AddUpdateServiceResponse<UserDTO>> AddUser(AddUserDTO newUser);
    }
}
