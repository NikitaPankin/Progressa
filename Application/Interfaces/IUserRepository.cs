using Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetUserByLoginAsync(string login);
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task<bool> CheckPasswordAsync(string login, string password);
    }
}
