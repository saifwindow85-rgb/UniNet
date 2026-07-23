using Contracts.Common.DTOs.User_Token_DTOs;
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

            foreach(var role in info.UserRoles)
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
