using Contracts.Responses.IdentityResponses;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.IdentityInterfaces.UserInterfaces
{
    public interface IUserRepository
    {
        public  Task<bool> IsUserExsist(string userName);
        public Task<UserDTO?> GetUserById(int Id);
        public Task<User?> GetUserEntityById(int Id);
        public Task<User?> GetUserByUserName(string userName);
        public Task Add(User user);
    }
}
