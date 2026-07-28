using Contracts.Requests.LoginRequests;
using DataAccessLayer.Dbcontext;
using Domain.Entities.Token;
using Domain.Interfaces.LoginInterfaces.TokenInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbcontext _context;
        private Expression<Func<RefreshToken, UserToken>> ToDetaieldTokenDTO = t => new UserToken
        {
            RefreshTokenId = t.RefreshTokenId,
            Type = t.User.Type,
            TokenHash = t.TokenHash,
            ExpiresAt = t.ExpiresAt,
            RevokedAt = t.RevokedAt,
            UserId = t.UserId,
            UserName = t.User.UserName,
            ReplacedByTokenId = t.ReplacedByTokenId,

        };
        public RefreshTokenRepository(AppDbcontext context)
        {
            _context = context;
        }
        public async Task Add(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task<RefreshToken?> GetRefreshTokenById(int tokenId)
        {
            return await _context.RefreshTokens.FindAsync(tokenId);
        }

        public async Task<UserToken?> GetTokenDetails(string refreshToken)
        {
            return await _context.RefreshTokens.Select(ToDetaieldTokenDTO).Where(r => r.TokenHash == refreshToken).SingleOrDefaultAsync();
        }


    }
}
