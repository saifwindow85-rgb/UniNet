using Contracts.Requests.LoginRequests;
using Domain.Entities.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.LoginInterfaces.TokenInterfaces
{
    public interface IRefreshTokenRepository
    {
        public Task Add(RefreshToken refreshToken);
        public Task<UserToken?> GetTokenDetails(string refreshToken);
        public Task<RefreshToken?> GetRefreshTokenById(int tokenId);
    }
}
