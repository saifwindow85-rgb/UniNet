using Contracts.Common.DTOs.User_Token_DTOs;
using Contracts.Requests.LoginRequests;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public static class TokenUserInfoMapper
    {
        public static TokenUserInfoDTO ToInfoDTO(this User user,List<string>userRoles)
        {
            return new TokenUserInfoDTO
            {
                UserId = user.UserId,
                UserName = user.UserName,
                UniversityId = user.UniversityId,
                UserRoles = userRoles
            };
        }

        public static TokenUserInfoDTO ToInfoDTO(this UserToken token,List<string>userRoles)
        {
            return new TokenUserInfoDTO
            {
                UserId = token.UserId,
                UserName = token.UserName,
                UniversityId= token.UniversityId,
                UserRoles = userRoles
            };
        }
    }
}
