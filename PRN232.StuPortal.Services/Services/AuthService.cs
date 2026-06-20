using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PRN232.StuPortal.Repositories.BusinessModels;
using PRN232.StuPortal.Repositories.Entities;
using PRN232.StuPortal.Repositories.Interfaces;
using PRN232.StuPortal.Services.Interfaces;
using PRN232.StuPortal.Services.Models.Requests;
using PRN232.StuPortal.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.StuPortal.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(
            IAuthRepository authRepository,
            IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _authRepository.GetUserByUsernameAsync(request.Username);
            if (user == null)
                return null;

            var entity = ToUserEntity(user);
            var result = _passwordHasher.VerifyHashedPassword(
                entity,
                user.PasswordHash,
                request.Password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            return await CreateAuthResponseAsync(user);
        }

        public async Task<AuthResponse?> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var refreshToken = await _authRepository
                .GetRefreshTokenByTokenAsync(request.RefreshToken);

            if (refreshToken?.User == null ||
                refreshToken.RevokedAt != null ||
                refreshToken.ExpiresAt <= DateTime.UtcNow)
                return null;

            await _authRepository.RevokeRefreshTokenAsync(request.RefreshToken);
            return await CreateAuthResponseAsync(refreshToken.User);
        }

        private async Task<AuthResponse> CreateAuthResponseAsync(UserBusinessModel user)
        {
            var expiresIn = GetExpiryMinutes() * 60;
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            await _authRepository.AddRefreshTokenAsync(new RefreshTokenBusinessModel
            {
                Token = refreshToken,
                UserId = user.UserId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = expiresIn
            };
        }

        private string GenerateAccessToken(UserBusinessModel user)
        {
            var secret = _configuration["Jwt:Secret"];
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("JWT secret is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(GetExpiryMinutes()),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }

        private int GetExpiryMinutes()
        {
            return int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var minutes)
                ? minutes
                : 60;
        }

        private static User ToUserEntity(UserBusinessModel user)
        {
            return new User
            {
                UserId = user.UserId,
                Username = user.Username,
                PasswordHash = user.PasswordHash,
                Role = user.Role
            };
        }
    }
}
