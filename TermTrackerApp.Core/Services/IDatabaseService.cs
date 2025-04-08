using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TermTrackerApp.Core.Models;

namespace TermTrackerApp.Core.Services
{
    public interface IDatabaseService
    {
        Task Init();
        Task AddUserAsync(User user);
        Task<User> GetUserAsync(string username);
        Task<int> SaveTermAsync(Term term);
        Task<int> SaveCourseAsync(Course course);
        Task<int> SavePAAsync(PerformanceAssessment assessment);
        Task<int> SaveOAAsync(ObjectiveAssessment assessment);
        Task UpdateTermAsync(Term term);
        Task UpdateCourseAsync(Course course);
        Task UpdateOAAsync(ObjectiveAssessment assessment);
        Task UpdatePAAsync(PerformanceAssessment assessment);
        Task<IEnumerable<Term>> GetTermsAsync(int userId);
        Task<Term> GetTermByIDAsync(int termId, int userId);
        Task<IEnumerable<Course>> GetCoursesAsync(int termId, int userId);
        Task<IEnumerable<Course>> GetCoursesByDateAsync(DateTime start, DateTime end, int userId);
        Task<IEnumerable<Course>> GetCoursesByNameAsync(string name, int userId);
        Task<IEnumerable<Assessment>> GetAssessmentsAsync(int courseId, int userId);
        Task<PerformanceAssessment> GetPAAsync(int assessmentId, int userId);
        Task<ObjectiveAssessment> GetOAAsync(int assessmentId, int userId);
        Task DeleteTermAsync(Term term);
        Task DeleteCourseAsync(int courseId);
        Task DeleteOAAsync(int assessmentID);
        Task DeletePAAsync(int assessmentID);

        Task<Assessment> GetAssessmentAsync(int id, int userId);
        Task<int> SaveAssessmentAsync(Assessment assessment);
        Task UpdateAssessmentAsync(Assessment assessment);
        Task DeleteAssessmentAsync(Assessment assessment);
    }
}
