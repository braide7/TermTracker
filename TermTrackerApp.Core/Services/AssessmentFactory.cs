using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TermTrackerApp.Core.Models;

namespace TermTrackerApp.Core.Services
{
    public static class AssessmentFactory
    {
        /// Creates a new assessment instance of the specified type
        public static Assessment CreateAssessment(int typeIndex)
        {
            switch (typeIndex)
            {
                case 0:
                    return new ObjectiveAssessment { TypeIndex = 0, Type = "Objective Assessment" };
                case 1:
                    return new PerformanceAssessment { TypeIndex = 1, Type = "Performance Assessment" };
                default:
                    throw new ArgumentException($"Unknown assessment type index: {typeIndex}");
            }
        }

        /// Creates a deep copy of an existing assessment
        public static Assessment CloneAssessment(Assessment source)
        {
            if (source is ObjectiveAssessment oa)
            {
                return new ObjectiveAssessment
                {
                    Id = oa.Id,
                    Name = oa.Name,
                    CourseId = oa.CourseId,
                    UserId = oa.UserId,
                    StartDate = oa.StartDate,
                    EndDate = oa.EndDate,
                    TypeIndex = oa.TypeIndex,
                    Type = oa.Type,
                    StartNotification = oa.StartNotification,
                    EndNotification = oa.EndNotification,
                    NumberOfQuestions = oa.NumberOfQuestions
                };
            }
            else if (source is PerformanceAssessment pa)
            {
                return new PerformanceAssessment
                {
                    Id = pa.Id,
                    Name = pa.Name,
                    CourseId = pa.CourseId,
                    UserId = pa.UserId,
                    StartDate = pa.StartDate,
                    EndDate = pa.EndDate,
                    TypeIndex = pa.TypeIndex,
                    Type = pa.Type,
                    StartNotification = pa.StartNotification,
                    EndNotification = pa.EndNotification,
                    TaskTypeIndex = pa.TaskTypeIndex,
                    TaskType = pa.TaskType
                };
            }

            throw new ArgumentException("Unknown assessment type");
        }
    }
}
