using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Entities;
using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> GetUserByNameAsync(string name)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.FullName == name);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<bool> CheckPasswordAsync(string login, string password)
        {
            var user = await GetUserByEmailAsync(login);
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<ApplicationUser> CreateUserAsync(string email, string name, string password)
        {
            var user = new ApplicationUser { Email = email, UserName = email, FullName = name };

            var existingUser = await _userManager.FindByNameAsync(user.UserName);
            if (existingUser == null)
            {
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    string? errors = null;
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine(error.Description);
                        errors += $"{error.Description}\n\r";
                    }
                    throw new Exception(errors);
                }
            }
            else
            {
                if (existingUser.FullName == name) throw new Exception("This name is already taken.");
                else throw new Exception("A user with this email address already exists.");
            }
            return await GetUserByNameAsync(name);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return WebUtility.UrlEncode(token);
        }

        public async Task<bool> ConfirmEmailAsync(ApplicationUser user, string token)
        {
            if (user == null || string.IsNullOrWhiteSpace(token)) return false;

            var decodedToken = WebUtility.UrlDecode(token);

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (result.Succeeded) 
            {
                _db.Users.FirstAsync(u => u.Id == user.Id).Result.EmailConfirmed = true;
                await _db.SaveChangesAsync();
            }
            return result.Succeeded;
        }
    }
}
