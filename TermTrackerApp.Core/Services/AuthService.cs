using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using TermTrackerApp.Core.Models;

namespace TermTrackerApp.Core.Services
{
    public class AuthService
    {
        private static int? _currentUserID;
        private readonly IUserRepository _userRepo;
        public int? CurrentUserId => _currentUserID;
        public AuthService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }
        public async Task<bool> RegisterUser(string username, string password)
        {


            // Check if user already exists
            var existingUser = await _userRepo.GetUser(username);
            if (existingUser != null) return false;

            // Hash password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User { Username = username, PasswordHash = passwordHash };
            await _userRepo.AddUser(user);

            _currentUserID = user.Id;
            return true;
        }

        public async Task<bool> LoginUser(string username, string password)
        {


            var user = await _userRepo.GetUser(username);
            if (user == null) return false; // User not found

            _currentUserID = user.Id;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

        }

        public async Task<bool> LogoutUser()
        {
            _currentUserID = null;

            return true;
        }
    }
}
