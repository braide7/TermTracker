using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TermTrackerApp.Core.Models
{
    public class Course
    {
        private int _id;
        private int _userId;
        private int _termId;
        private string _name;
        private int _statusIndex;
        private string _status;
        private DateTime _startDate;
        private DateTime _endDate;
        private string _instructorName;
        private string _instructorPhone;
        private string _instructorEmail;
        private bool _startNotification;
        private bool _endNotification;
        private string _notes;
        private List<Assessment> _assessments;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        [ForeignKey(typeof(User))]
        public int UserId { get => _userId; set => _userId = value; }

        [ForeignKey(typeof(Term))]
        public int TermId
        {
            get => _termId;
            set => _termId = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public int StatusIndex
        {
            get => _statusIndex;
            set => _statusIndex = value;
        }

        public string Status
        {
            get => _status;
            set => _status = value;
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

        public string InstructorName
        {
            get => _instructorName;
            set => _instructorName = value;
        }

        public string InstructorPhone
        {
            get => _instructorPhone;
            set => _instructorPhone = value;
        }

        public string InstructorEmail
        {
            get => _instructorEmail;
            set => _instructorEmail = value;
        }

        public bool StartNotification
        {
            get => _startNotification;
            set => _startNotification = value;
        }

        public bool EndNotification
        {
            get => _endNotification;
            set => _endNotification = value;
        }

        public string Notes
        {
            get => _notes;
            set => _notes = value;
        }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Assessment> Assessments
        {
            get => _assessments ??= new List<Assessment>();
            private set => _assessments = value;
        }

        public Course()
        {
            _assessments = new List<Assessment>();
        }

    }
}

