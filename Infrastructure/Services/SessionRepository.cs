using Application.Entities;
using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ISecurityKeyManager _securityKeyManager;

        public SessionRepository(ApplicationDbContext db, ISecurityKeyManager securityKeyManager, IUserRepository userRepository)
        {
            _db = db;
            _securityKeyManager = securityKeyManager;
        }

        public async Task<(JwtSecurityToken accessToken, string refreshToken)> CreateAccessRefreshTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("FullName", user.FullName)
            };

            var accessToken = new JwtSecurityToken(
               issuer: _securityKeyManager.Issuer,
               audience: _securityKeyManager.Audience,
               claims: claims,
               expires: DateTime.UtcNow.AddMinutes(1),
               signingCredentials: new SigningCredentials(_securityKeyManager.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

            var refreshToken = Guid.NewGuid().ToString("N");
            var refreshHash = TokenHelpers.ComputeSha256Hash(refreshToken);

            var session = new UserSession
            {
                UserId = user.Id,
                RefreshTokenHash = refreshHash,
                RefreshUpdatedAtUtc = DateTime.UtcNow
            };

            _db.UserSessions.Add(session);
            await _db.SaveChangesAsync();

            return await Task.FromResult((accessToken, refreshToken));
        }

        public async Task<(JwtSecurityToken accessToken, string refreshToken)> ExtendSessionByRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken)) throw new SecurityTokenException("Invalid refresh token");

            var refreshHash = TokenHelpers.ComputeSha256Hash(refreshToken);

            var session = await _db.UserSessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.RefreshTokenHash == refreshHash);

            if (session == null) throw new SecurityTokenException("Invalid refresh token");

            if (session.RefreshUpdatedAtUtc < DateTime.UtcNow.AddDays(-10))
            {
                _db.UserSessions.Remove(session);
                await _db.SaveChangesAsync();
                throw new SecurityTokenException("Invalid refresh token");
            }

            var newRefreshToken = Guid.NewGuid().ToString("N");
            session.RefreshTokenHash = TokenHelpers.ComputeSha256Hash(newRefreshToken);
            session.RefreshUpdatedAtUtc = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, session.User.UserName),
                new Claim("FullName", session.User.FullName)
            };

            var accessToken = new JwtSecurityToken(
               issuer: "MyAuthServer",
               audience: "MyApiClient",
               claims: claims,
               expires: DateTime.UtcNow.AddMinutes(1),
               signingCredentials: new SigningCredentials(_securityKeyManager.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

            await _db.SaveChangesAsync();

            return (accessToken, newRefreshToken);
        }
    }
}
