using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TermTrackerApp.Core.Models;
using TermTrackerApp.Core.Services;

namespace TermTrackerApp.Core.Services
{

    public class UserRepository : IUserRepository
    {
        private readonly IDatabaseService _databaseService;

        public UserRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        public async Task<User> GetUser(string username)
        {
            return await _databaseService.GetUserAsync(username);
        }

        public async Task AddUser(User user)
        {
            await _databaseService.AddUserAsync(user);
        }
    }
}
