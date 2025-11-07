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
        Task<ApplicationUser> GetUserByNameAsync(string name);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task<bool> CheckPasswordAsync(string login, string password);
        Task<ApplicationUser> CreateUserAsync(string email, string name, string password);
        Task<bool> ConfirmEmailAsync(ApplicationUser user, string token);
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
    }
}
