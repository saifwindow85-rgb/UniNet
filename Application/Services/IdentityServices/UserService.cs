using Domain.Interfaces.UnitOfWork;
using Domain.Interfaces.UserInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Application.Services.IdentityServices
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWorkRepository _unitOfWork;
        public UserService(IUnitOfWorkRepository unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
