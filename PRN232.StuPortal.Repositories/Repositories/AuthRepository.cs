using Microsoft.EntityFrameworkCore;
using PRN232.StuPortal.Repositories.BusinessModels;
using PRN232.StuPortal.Repositories.Data;
using PRN232.StuPortal.Repositories.Entities;
using PRN232.StuPortal.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.StuPortal.Repositories.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<UserBusinessModel?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Where(x => x.Username == username)
                .Select(x => new UserBusinessModel
                {
                    UserId = x.UserId,
                    Username = x.Username,
                    PasswordHash = x.PasswordHash,
                    Role = x.Role
                })
                .FirstOrDefaultAsync();
        }

        public async Task AddRefreshTokenAsync(RefreshTokenBusinessModel refreshToken)
        {
            _context.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken.Token,
                ExpiresAt = refreshToken.ExpiresAt,
                CreatedAt = refreshToken.CreatedAt,
                UserId = refreshToken.UserId
            });

            await _context.SaveChangesAsync();
        }

        public async Task<RefreshTokenBusinessModel?> GetRefreshTokenByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(x => x.User)
                .Where(x => x.Token == token)
                .Select(x => new RefreshTokenBusinessModel
                {
                    RefreshTokenId = x.RefreshTokenId,
                    Token = x.Token,
                    ExpiresAt = x.ExpiresAt,
                    CreatedAt = x.CreatedAt,
                    RevokedAt = x.RevokedAt,
                    UserId = x.UserId,
                    User = new UserBusinessModel
                    {
                        UserId = x.User.UserId,
                        Username = x.User.Username,
                        PasswordHash = x.User.PasswordHash,
                        Role = x.User.Role
                    }
                })
                .FirstOrDefaultAsync();
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == token);

            if (refreshToken == null)
                return;

            refreshToken.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
