using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISecurityKeyManager
    {
        string Issuer { get; }
        string Audience { get; }
        SymmetricSecurityKey GetSymmetricSecurityKey();
    }
}
