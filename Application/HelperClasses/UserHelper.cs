using Contracts.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.HelperClasses
{
    public static  class UserHelper
    {
        public  static CurrentUserServiceResult GetIdFromCurrentUser(string ?userId)
        {
            var validUserId = -1;
            if (userId == null)
                return new CurrentUserServiceResult
                {
                    IsSuccess = false
                };

            if(int.TryParse(userId,out int Id))
            {
                validUserId = Id;
                return new CurrentUserServiceResult
                {
                    UserId = validUserId,
                    IsSuccess = true
                };
            }

            return new CurrentUserServiceResult
            {
                IsSuccess = false
            };
        }
    }
}
