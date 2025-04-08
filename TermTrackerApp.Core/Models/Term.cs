using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;


namespace TermTrackerApp.Core.Models
{
    public class Term
    {
        private int _id;
        private int _userId;
        private string _title;
        private DateTime _startDate;
        private DateTime _endDate;
        private List<Course> _courses;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        [ForeignKey(typeof(User))]
        public int UserId { get => _userId; set => _userId = value; }

        public string Title
        {
            get => _title;
            set => _title = value;
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => _startDate = value;
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => _endDate = value;
        }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Course> Courses
        {
            get => _courses ??= new List<Course>();
            set => _courses = value;
        }

        public Term()
        {
            _courses = new List<Course>();
        }

    }
}
