using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TermTrackerApp.Core.Models;

namespace TermTrackerApp.Core.Services
{
    public interface IUserRepository
    {
        Task<User> GetUser(string username);
        Task AddUser(User user);
    }
}
