using Application.Entities;
using Application.Interfaces;
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
        private readonly ISecurityKeyManager _securityKeyManager;

        public SessionRepository(ISecurityKeyManager securityKeyManager)
        {
            _securityKeyManager = securityKeyManager;
        }

        public Task<(JwtSecurityToken accessToken, JwtSecurityToken refreshToken)> CreateAccessRefreshTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim("FullName", user.FullName)
            };

            var symmetricSecurityKey = _securityKeyManager.GetSymmetricSecurityKey();
            var accessToken = new JwtSecurityToken(
               issuer: "MyAuthServer",
               audience: "MyApiClient",
               claims: claims,
               expires: DateTime.UtcNow.AddHours(1),
               signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
            );

            var refreshToken = new JwtSecurityToken(
               issuer: "MyAuthServer",
               audience: "MyApiClient",
               claims: claims,
               expires: DateTime.UtcNow.AddDays(7),
               signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
            );
            return Task.FromResult((accessToken, refreshToken));
        }

        public Task<(JwtSecurityToken accessToken, JwtSecurityToken refreshToken)> ExtendSessionByRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken)) throw new SecurityTokenException("Invalid refresh token");

            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken? validatedJwtToken;

            try
            {
                var principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _securityKeyManager.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _securityKeyManager.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _securityKeyManager.GetSymmetricSecurityKey(),
                    ValidateLifetime = true,
                }, out SecurityToken validatedToken);

                validatedJwtToken = (JwtSecurityToken?)validatedToken;
            }
            catch
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            var login = validatedJwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var fullName = validatedJwtToken?.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;

            if (login == null || fullName == null) throw new SecurityTokenException("Invalid refresh token claims");

            var user = new User { Login = login, FullName = fullName };

            return CreateAccessRefreshTokenAsync(user);
        }
    }
}
