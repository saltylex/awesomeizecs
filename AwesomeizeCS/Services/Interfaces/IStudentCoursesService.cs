using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;

namespace AwesomeizeCS.Services.Interfaces
{
    public interface IStudentCoursesService
    {
        Task CreateStudentCourse(StudentCourse studentCourse);
        Task CreateStudentCourse(List<StudentCourseViewModel> studentCourse);
        Task DeleteStudentCourse(StudentCourse studentCourse);
        Task<List<StudentCourse>> GetAllStudentCourses();
        Task<List<StudentCourse>> GetAllStudentCoursesByCourse(Course course);
        Task<List<Student>> GetAllStudents();
        Task<List<Course>> GetAllCourses();
        Task<StudentCourse?> GetStudentCourseById(Guid? id);
        Task<StudentCourse?> GetStudentCourseOrDefault(Guid? id);
        Task<List<StudentCourse>> GetStudentCoursesByActivity(TimeTable activity);
        Task<List<StudentCourse>> GetStudentCourseAndDetailsByEmail(string email);
        bool StudentCourseExists(Guid id);
        Task UpdateStudentCourse(StudentCourse studentCourse);
    }
}