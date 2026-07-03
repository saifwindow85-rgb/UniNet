using Contracts.Requests.LoginRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.LoginInterfaces.TokenInterfaces
{
    public interface IRefreshTokenService
    {
        public Task AddRefreshToken(string refreshToken, int UserId);
        public string GenerateRefreshToken();
        public Task<UserToken> GetTokenDetails(string userName);
        public Task RefreshToken(string refreshedToken, int userId, int replacedTokenId);

        public string GetHash(string refreshToken);
        public Task Logout(int tokenId);
    }
}
