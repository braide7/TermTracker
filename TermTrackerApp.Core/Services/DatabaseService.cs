using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Extensions;
using TermTrackerApp.Core.Models;

namespace TermTrackerApp.Core.Services
{

    public class DatabaseService : IDatabaseService
    {
        private SQLiteAsyncConnection _db;
        private readonly string _dbPath;
        private bool _isInitialized;

        public DatabaseService(string dbPath)
        {
            _dbPath = dbPath;
            _isInitialized = false;
        }

        public async Task Init()
        {
            if (_isInitialized)
            {
                return;
            }

            _db = new SQLiteAsyncConnection(_dbPath);

            await _db.CreateTableAsync<User>();
            await _db.CreateTableAsync<Term>();
            await _db.CreateTableAsync<Course>();
            await _db.CreateTableAsync<ObjectiveAssessment>();
            await _db.CreateTableAsync<PerformanceAssessment>();

            _isInitialized = true;
        }

        // Helper method to ensure DB is initialized before operations
        private async Task CheckInit()
        {
            if (!_isInitialized)
            {
                await Init();
            }
        }

        public async Task AddUserAsync(User user)
        {
            await CheckInit();
            await _db.InsertAsync(user);
        }

        public async Task<User> GetUserAsync(string username)
        {
            await CheckInit();
            return await _db.Table<User>().Where(i => i.Username == username).FirstOrDefaultAsync();
        }

        public async Task<int> SaveTermAsync(Term term)
        {
            await CheckInit();
            await _db.InsertAsync(term);
            return term.Id;
        }

        public async Task<int> SaveCourseAsync(Course course)
        {
            await CheckInit();
            await _db.InsertAsync(course);
            return course.Id;
        }

        public async Task<int> SavePAAsync(PerformanceAssessment assessment)
        {
            await CheckInit();
            // Basic validation
            if (string.IsNullOrWhiteSpace(assessment.Name))
                throw new ArgumentException("Assessment name cannot be empty");

            if (assessment.UserId <= 0)
                throw new ArgumentException("Invalid user ID");

            if (assessment.TaskTypeIndex < 0)
                throw new ArgumentException("Task type must be selected");

            await _db.InsertAsync(assessment);
            return assessment.Id;
        }

        public async Task<int> SaveOAAsync(ObjectiveAssessment assessment)
        {
            await CheckInit();
            // Basic validation to prevent invalid data
            if (string.IsNullOrWhiteSpace(assessment.Name))
                throw new ArgumentException("Assessment name cannot be empty");

            if (assessment.UserId <= 0)
                throw new ArgumentException("Invalid user ID");

            if (assessment.NumberOfQuestions <= 0)
                throw new ArgumentException("Number of questions must be positive");

            // The actual save operation uses parameters automatically
            await _db.InsertAsync(assessment);
            return assessment.Id;
        }

        public async Task UpdateTermAsync(Term term)
        {
            await CheckInit();
            await _db.UpdateAsync(term);
        }

        public async Task UpdateCourseAsync(Course course)
        {
            await CheckInit();
            await _db.UpdateAsync(course);
        }

        public async Task UpdateOAAsync(ObjectiveAssessment assessment)
        {
            await CheckInit();
            await _db.UpdateAsync(assessment);
        }

        public async Task UpdatePAAsync(PerformanceAssessment assessment)
        {
            await CheckInit();
            await _db.UpdateAsync(assessment);
        }

        public async Task<IEnumerable<Term>> GetTermsAsync(int userId)
        {
            await CheckInit();
            return await _db.Table<Term>().Where(i => i.UserId == userId).ToListAsync();
        }

        public async Task<Term> GetTermByIDAsync(int termId, int userId)
        {
            await CheckInit();
            return await _db.Table<Term>().Where(i => i.Id == termId && i.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync(int termId, int userId)
        {
            await CheckInit();
            return await _db.Table<Course>().Where(i => i.TermId == termId && i.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByDateAsync(DateTime start, DateTime end, int userId)
        {
            await CheckInit();
            return await _db.Table<Course>().Where(i => i.StartDate >= start && i.EndDate <= end && i.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByNameAsync(string name, int userId)
        {
            await CheckInit();
            return await _db.Table<Course>()
            .Where(i => i.Name.ToLower().Contains(name.ToLower()) && i.UserId == userId)
            .ToListAsync();
        }

        public async Task<IEnumerable<Assessment>> GetAssessmentsAsync(int courseId, int userId)
        {
            await CheckInit();
            var OA = await _db.Table<ObjectiveAssessment>().Where(i => i.CourseId == courseId && i.UserId == userId).ToListAsync();
            var PA = await _db.Table<PerformanceAssessment>().Where(i => i.CourseId == courseId && i.UserId == userId).ToListAsync();

            var allAssessments = new List<Assessment>();
            allAssessments.AddRange(OA);
            allAssessments.AddRange(PA);

            return allAssessments;
        }

        public async Task<PerformanceAssessment> GetPAAsync(int assessmentId, int userId)
        {
            await CheckInit();
            return await _db.Table<PerformanceAssessment>().Where(i => i.Id == assessmentId && i.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<ObjectiveAssessment> GetOAAsync(int assessmentId, int userId)
        {
            await CheckInit();
            return await _db.Table<ObjectiveAssessment>().Where(i => i.Id == assessmentId && i.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task DeleteTermAsync(Term term)
        {
            await CheckInit();

            try
            {
                var courses = await GetCoursesAsync(term.Id, term.UserId);
                var assessments = new List<Assessment>();

                foreach (Course course in courses)
                {
                    assessments.AddRange(await GetAssessmentsAsync(course.Id, course.UserId));
                }

                foreach (Assessment assessment in assessments)
                {
                    if (assessment is PerformanceAssessment)
                    {
                        await DeletePAAsync(assessment.Id);
                    }
                    else if (assessment is ObjectiveAssessment)
                    {
                        await DeleteOAAsync(assessment.Id);
                    }
                }

                foreach (Course course in courses)
                {
                    await DeleteCourseAsync(course.Id);
                }

                await _db.DeleteAsync<Term>(term.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }

        public async Task DeleteCourseAsync(int courseId)
        {
            await CheckInit();
            await _db.DeleteAsync<Course>(courseId);
        }

        public async Task DeleteOAAsync(int assessmentID)
        {
            await CheckInit();
            await _db.DeleteAsync<ObjectiveAssessment>(assessmentID);
        }

        public async Task DeletePAAsync(int assessmentID)
        {
            await CheckInit();
            await _db.DeleteAsync<PerformanceAssessment>(assessmentID);
        }

        public async Task<Assessment> GetAssessmentAsync(int id, int userId)
        {
            await CheckInit();

            // Try to get as ObjectiveAssessment first
            var oa = await _db.Table<ObjectiveAssessment>()
             .Where(a => a.Id == id && a.UserId == userId)
             .FirstOrDefaultAsync();

            if (oa != null)
                return oa;

            var pa = await _db.Table<PerformanceAssessment>()
                .Where(a => a.Id == id && a.UserId == userId)
                .FirstOrDefaultAsync();

            return pa;
        }

        public async Task<int> SaveAssessmentAsync(Assessment assessment)
        {
            await CheckInit();

            if (assessment is ObjectiveAssessment oa)
            {
                return await SaveOAAsync(oa);
            }
            else if (assessment is PerformanceAssessment pa)
            {
                return await SavePAAsync(pa);
            }

            throw new ArgumentException("Unknown assessment type");
        }

        public async Task UpdateAssessmentAsync(Assessment assessment)
        {
            await CheckInit();

            if (assessment is ObjectiveAssessment oa)
            {
                await UpdateOAAsync(oa);
            }
            else if (assessment is PerformanceAssessment pa)
            {
                await UpdatePAAsync(pa);
            }
            else
            {
                throw new ArgumentException("Unknown assessment type");
            }
        }

        public async Task DeleteAssessmentAsync(Assessment assessment)
        {
            await CheckInit();

            if (assessment is ObjectiveAssessment)
            {
                await DeleteOAAsync(assessment.Id);
            }
            else if (assessment is PerformanceAssessment)
            {
                await DeletePAAsync(assessment.Id);
            }
            else
            {
                throw new ArgumentException("Unknown assessment type");
            }
        }
    }

}
