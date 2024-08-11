using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using AwesomeizeCS.Repositories.Interfaces;
using AwesomeizeCS.Services.Interfaces;
using System.Text.RegularExpressions;

namespace AwesomeizeCS.Services
{
    public class StudentCoursesService : IStudentCoursesService
    {
        private readonly IStudentCoursesRepository _repository;
        public StudentCoursesService(IStudentCoursesRepository repository)
        {
            _repository = repository;
        }

        public Task<StudentCourse?> GetStudentCourseById(Guid? id)
        {
            return _repository.GetStudentCourseById(id);
        }
        public Task<StudentCourse?> GetStudentCourseOrDefault(Guid? id)
        {
            return _repository.GetStudentCourseOrDefault(id);
        }

        public Task<List<StudentCourse>> GetStudentCoursesByActivity(TimeTable activity)
        {
            return _repository.GetStudentCoursesByActivity(activity);
        }

        public Task<List<StudentCourse>> GetAllStudentCourses()
        {
            return _repository.GetAllStudentCourses();
        }

        public Task<List<StudentCourse>> GetAllStudentCoursesByCourse(Course course)
        {
            return _repository.GetAllStudentCoursesByCourse(course);
        }

        public Task<List<StudentCourse>> GetStudentCourseAndDetailsByEmail(string email)
        {
            return _repository.GetStudentCourseAndDetailsByEmail(email);
        }

        public Task<List<Student>> GetAllStudents()
        {
            return _repository.GetAllStudents();
        }

        public Task<List<Course>> GetAllCourses()
        {
            return _repository.GetAllCourses();
        }

        public bool StudentCourseExists(Guid id)
        {
            return _repository.StudentCourseExists(id);
        }

        public async Task CreateStudentCourse(StudentCourse studentCourse)
        {

            string pattern = @"[0-9]{3}-[12]$";
                if (!Regex.IsMatch(studentCourse.AttendingGroup, pattern) )
            {
                var fieldName = nameof(studentCourse.AttendingGroup);
                var errorMessage = "Invalid Attending group";
                throw new ArgumentException($"Field: {fieldName}, Error: {errorMessage}");
            }

            studentCourse.Course = await _repository.GetCourseById(studentCourse.Course.Id);
            if (studentCourse.Course == null)
            {
                var fieldName = nameof(studentCourse.Course);
                var errorMessage = "Invalid course data";
                throw new ArgumentException($"Field: {fieldName}, Error: {errorMessage}");
            }
            studentCourse.Student = await _repository.GetStudentById(studentCourse.Student.Id);
            if (studentCourse.Student == null)
            {
                var fieldName = nameof(studentCourse.Student);
                var errorMessage = "Invalid student data";
                throw new ArgumentException($"Field: {fieldName}, Error: {errorMessage}");
            }
            await _repository.CreateStudentCourse(studentCourse);
        }

        public async Task UpdateStudentCourse(StudentCourse studentCourse)
        {
            string pattern = @"[0-9]{3}-[12]$";
            if (!Regex.IsMatch(studentCourse.AttendingGroup, pattern))
            {
                var fieldName = nameof(studentCourse.AttendingGroup);
                var errorMessage = "Invalid Attending group";
                throw new ArgumentException($"Field: {fieldName}, Error: {errorMessage}");
            }
            await _repository.UpdateStudentCourse(studentCourse);
        }

        public async Task DeleteStudentCourse(StudentCourse studentCourse)
        {
            await _repository.DeleteStudentCourse(studentCourse);
        }

        public async Task CreateStudentCourse(List<StudentCourseViewModel> studentCourse)
        {
            await _repository.CreateStudentCourse(studentCourse);
        }
    }
}
