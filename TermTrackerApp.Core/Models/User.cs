using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermTrackerApp.Core.Models
{
    public class User
    {
        private int _id;
        private string _username;
        private string _passwordHash; // Store hashed passwords, not plain text

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string Username
        {
            get => _username;
            set => _username = value;
        }

        public string PasswordHash
        {
            get => _passwordHash;
            set => _passwordHash = value;
        }
    }
}
