using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using AwesomeizeCS.Repositories.Interfaces;
using AwesomeizeCS.Services.Interfaces;

namespace AwesomeizeCS.Services
{
    public class CoursesService : ICoursesService
    {
        private readonly ICoursesRepository _repository;
        public CoursesService(ICoursesRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Course>> GetCoursesByMainTeacher(Guid id)
        {
            return _repository.GetCoursesByMainTeacher(id);
        }

        public ValueTask<Course?> GetCourseById(Guid? id)
        {
            return _repository.GetCourseById(id);
        }
        public Task<Course?> GetCourseOrDefault(Guid? id)
        {
            return _repository.GetCourseOrDefault(id);
        }

        public Task<List<CoursesTeacherNameViewModel>> GetAllCourses()
        {
            return _repository.GetAllCourses();
        }

        public bool CourseExists(Guid id)
        {
            return _repository.CourseExists(id);
        }

        public async Task CreateCourse(Course course)
        {
            ValidateCourse(course);

            await _repository.CreateCourse(course);
        }

        public async Task UpdateCourse(Course course)
        {
            ValidateCourse(course);

            await _repository.UpdateCourse(course);
        }

        public async Task DeleteCourse(Course course)
        {

            await _repository.DeleteCourse(course);
        }

        private void ValidateCourse(Course course)
        {

            if (course.NumberOfSeminars < 0)
            {
                var fieldName = nameof(course.NumberOfSeminars);
                var errorMessage = " Number of seminars must be a positive number.";
                throw new ArgumentException($"Field: {fieldName}, Error: {errorMessage}");
            }

            if (course.NumberOfLabs < 0)
            {
                var fieldName = nameof(course.NumberOfLabs);
                var errorMessage = " Number of Labs must be a positive number.";
                throw new ArgumentException($"Field: {fieldName}, Error: {errorMessage}");
            }

            if (course.NumberOfCourses < 0)
            {
                var fieldName = nameof(course.NumberOfCourses);
                var errorMessage = " Number of courses must be a positive number.";
                throw new ArgumentException($"Field: {fieldName}, Error: {errorMessage}");
            }
            bool isValidYear = int.TryParse(course.AcademicYear, out int year) &&
                               year >= 2023 &&
                               year <= 2100;
            if (!isValidYear)
            {
                var fieldName = nameof(course.AcademicYear);
            
                var errorMessage = "The year is invalid.";
                throw new ArgumentException($"Field: {fieldName}, Error: {errorMessage}");
            }

        }

        public Task<List<StudentSituationViewModel>> GetStudentSituationData()
        {
            return _repository.GetStudentSituationData();
        }
    }
}
