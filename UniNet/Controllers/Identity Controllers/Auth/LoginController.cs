using Contracts.Requests.LoginRequests;
using Contracts.Responses.Login;
using DataAccessLayer.Configurations.Options;
using Domain.Interfaces.LoginInterfaces.TokenInterfaces;
using Domain.Interfaces.UserInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UniNet.Controllers.Identity_Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly JWTOption _jwtOption;

        public LoginController(IUserService userService, IRefreshTokenService refreshTokenService, IOptions<JWTOption> jwtOption)
        {
            _userService = userService;
            _refreshTokenService = refreshTokenService;
            _jwtOption = jwtOption.Value;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Login([FromBody]LoginRequest request)
        {
            var user = await _userService.FindByUserName(request.UserName);
            if(user == null)
            {
                return Unauthorized("Invalid credentials!");
            }

            if (!_userService.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials!");
            }

            if(!user.IsActive)
            {
                return StatusCode(403, new { Title = "Banned Account", Message = "Your Account Is Banned" });
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                new Claim(ClaimTypes.Name,user.UserName),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.Key));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                  issuer: "UniNetAPI",
                  audience: "UniNetAPIUsers",
                  claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: creds
                );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = _refreshTokenService.GenerateRefreshToken();
            await _refreshTokenService.AddRefreshToken(refreshToken, user.UserId);
            return Ok(new TokenResponse
            {
                AccesseToken = accessToken,
                RefreshToken = refreshToken
            });
        }

    }
}
