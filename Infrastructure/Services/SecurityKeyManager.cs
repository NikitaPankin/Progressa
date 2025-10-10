using Application.Interfaces;
using Infrastructure.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SecurityKeyManager : ISecurityKeyManager
    {
        private readonly AuthenticationSettings _jwtOptions;

        public SecurityKeyManager(IOptions<AuthenticationSettings> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string Issuer => _jwtOptions.Issuer;

        public string Audience => _jwtOptions.Audience;

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.PrivateKey));
        }
    }
}
