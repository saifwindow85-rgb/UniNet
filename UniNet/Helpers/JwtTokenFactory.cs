using Contracts.Common.DTOs.User_Token_DTOs;
using Contracts.Enums;
using Contracts.Requests.LoginRequests;
using DataAccessLayer.Configurations.Options;
using Domain.Entities.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UniNet.CustomClaims;

namespace UniNet.Helpers
{
    public class JwtTokenFactory
    {
        public static JwtSecurityToken TokenIssuer(TokenUserInfoDTO info,JWTOption jwtOptions)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, info.UserId.ToString()),
                new Claim(ClaimTypes.Name, info.UserName), 

            };
            
            if(info.UniversityId != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UnivresityId, info.UniversityId.ToString()!));
            }

            if(info.CollegeId.HasValue)
            {
                claims.Add(new Claim(CustomClaimTypes.CollegeId, info.CollegeId.ToString()!));
            }

            if(info.DepartmentId.HasValue)
            {
                claims.Add(new Claim(CustomClaimTypes.DepartmentId, info.DepartmentId.ToString()!));
            }

            if(info.BatchId.HasValue)
            {
                claims.Add(new Claim(CustomClaimTypes.BatchId, info.BatchId.ToString()!));
            }
            if(info.StudentId.HasValue)
            {
                claims.Add(new Claim(CustomClaimTypes.StudentId, info.StudentId.ToString()!));
            }

            if(info.EmployeeId.HasValue)
            {
                claims.Add(new Claim(CustomClaimTypes.EmployeeId, info.EmployeeId.ToString()!));
            }

            if(info.Type == UserType.SystemAdmin || info.Type == UserType.Student|| info.Type == UserType.Employee)
            {
                claims.Add(new Claim(CustomClaimTypes.UserType, info.Type.ToString()));
            }
            else
            {
                throw new ArgumentNullException("Type cannnot be null!");
            }

            foreach (var role in info.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                  issuer: jwtOptions.Issuer,
                  audience: jwtOptions.Audience,
                  claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(jwtOptions.AccessTokenLifeTimeInMinutes),
                    signingCredentials: creds
                );
            return token;
        }
    }
}
