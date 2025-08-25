using Application.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISessionRepository
    {
        Task<(JwtSecurityToken accessToken, JwtSecurityToken refreshToken)> CreateAccessRefreshTokenAsync(User user);
        Task<(JwtSecurityToken accessToken, JwtSecurityToken refreshToken)> ExtendSessionByRefreshTokenAsync(string refreshToken);
    }
}
