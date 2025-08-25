using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Entities;
using Application.Interfaces;

namespace Infrastructure.Services
{
    public class UserRepository : IUserRepository
    {
        public Task<User> GetUserByLoginAsync(string login)
        {
            return Task.FromResult(new User
            {
                Login = login,
                FullName = "Иван Иванович"
            });
        }

        public Task<User> GetUserByIdAsync(int id)
        {
            return Task.FromResult(new User
            {
                Login = "user",
                FullName = "Пётр Петрович"
            });
        }

        public Task<bool> CheckPasswordAsync(string login, string password)
        {
            return Task.FromResult(password == "123");
        }
    }
}
