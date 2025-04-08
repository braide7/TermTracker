using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermTrackerApp.Core.Models
{

    public abstract class Assessment
    {
        private int _id;
        private int _userId;
        private int _courseId;
        private string _name;
        private int _typeIndex;
        private string _type;
        private DateTime _startDate;
        private DateTime _endDate;
        private bool _startNotification;
        private bool _endNotification;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        [ForeignKey(typeof(User))]
        public int UserId { get => _userId; set => _userId = value; }

        [ForeignKey(typeof(Course))]
        public int CourseId
        {
            get => _courseId;
            set => _courseId = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public int TypeIndex
        {
            get => _typeIndex;
            set => _typeIndex = value;
        }

        public string Type
        {
            get => _type;
            set => _type = value;
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

    }
}
