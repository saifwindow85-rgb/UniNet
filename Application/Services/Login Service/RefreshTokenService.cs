using Contracts.Requests.LoginRequests;
using Domain.Entities.Token;
using Domain.Interfaces.LoginInterfaces.TokenInterfaces;
using Domain.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Login_Service
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        public RefreshTokenService(IUnitOfWorkRepository unitOfWorkRepository)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
        }
        public async Task AddRefreshToken(string refreshToken, int UserId)
        {
            using var sha = SHA256.Create();
            var incommingHash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(refreshToken)));
            var refreshTokenEntity = new RefreshToken
            {
                TokenHash = incommingHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                RevokedAt = null,
                UserId = UserId,
            };
            await _unitOfWorkRepository.RefreshTokenRepository.Add(refreshTokenEntity);
            await _unitOfWorkRepository.CompleteAsync();
        }

        public string GenerateRefreshToken()
        {
            var bytes = new byte[64];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            return Convert.ToBase64String(bytes);
        }

        public string GetHash(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public async Task<UserToken?> GetTokenDetails(string tokenHash)
        {
            using var sha = SHA256.Create();
            var incommingHash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(tokenHash)));
            return  await _unitOfWorkRepository.RefreshTokenRepository.GetTokenDetails(incommingHash);
        }

        public async Task Logout(int tokenId)
        {
            var refrshToken = await _unitOfWorkRepository.RefreshTokenRepository.GetRefreshTokenById(tokenId);
            refrshToken!.RevokedAt = DateTime.UtcNow;
            await _unitOfWorkRepository.CompleteAsync();
        }

        public async Task RefreshToken(string refreshedToken, int userId, int replacedTokenId)
        {
            using var sha = SHA256.Create();
            var incommingHash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(refreshedToken)));

            var refreshTokenEntity = new RefreshToken
            {
                TokenHash = incommingHash,
                UserId = userId,
                ReplacedByTokenId = replacedTokenId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            var oldRefreshToken = await _unitOfWorkRepository.RefreshTokenRepository.GetRefreshTokenById(replacedTokenId);
            oldRefreshToken!.RevokedAt = DateTime.UtcNow;
            await _unitOfWorkRepository.RefreshTokenRepository.Add(refreshTokenEntity);
            await _unitOfWorkRepository.CompleteAsync();
        }
    }
}
