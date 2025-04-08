using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TermTrackerApp.Core.Models
{

    public class PerformanceAssessment : Assessment
    {
        private int _taskTypeIndex;
        private string _taskType;

        public int TaskTypeIndex
        {
            get => _taskTypeIndex;
            set => _taskTypeIndex = value;
        }

        public string TaskType
        {
            get => _taskType;
            set => _taskType = value;
        }

        public PerformanceAssessment()
        {
            TypeIndex = 1;
            Type = "PA";

        }
    }
}
