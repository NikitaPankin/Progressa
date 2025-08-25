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
        Task<User> GetUserByLoginAsync(string login);
        Task<User> GetUserByIdAsync(int id);
        Task<bool> CheckPasswordAsync(string login, string password);
    }
}
