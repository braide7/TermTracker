using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TermTrackerApp.Core.Models
{

    public class ObjectiveAssessment : Assessment
    {


        private int _numberOfQuestions;


        public int NumberOfQuestions
        {
            get => _numberOfQuestions;
            set => _numberOfQuestions = value;
        }

        public ObjectiveAssessment()
        {
            TypeIndex = 0;
            Type = "OA";

        }

    }
}
