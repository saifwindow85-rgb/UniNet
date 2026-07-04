using Contracts.Common.DTOs.User_Token_DTOs;
using Contracts.Requests.LoginRequests;
using DataAccessLayer.Configurations.Options;
using Domain.Entities.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UniNet.Helpers
{
    public class AuthenticationHelper
    {
        public static JwtSecurityToken TokenIssuer(TokenUserInfoDTO info,JWTOption jwtOptions)
        {
            var claims = new[]
          {
                new Claim(ClaimTypes.NameIdentifier,info.UserId.ToString()),
                new Claim(ClaimTypes.Name,info.UserName),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                  issuer: jwtOptions.Issuer,
                  audience: jwtOptions.Audience,
                  claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: creds
                );
            return token;
        }
    }
}
