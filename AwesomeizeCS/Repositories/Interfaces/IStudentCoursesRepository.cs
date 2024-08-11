using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;

namespace AwesomeizeCS.Repositories.Interfaces;

public interface IStudentCoursesRepository
{
    Task<StudentCourse?> GetStudentCourseById(Guid? id);
    ValueTask<Student> GetStudentById(Guid? id);
    ValueTask<Course> GetCourseById(Guid? id);
    Task<StudentCourse?> GetStudentCourseOrDefault(Guid? id);
    Task<List<StudentCourse>> GetStudentCoursesByActivity(TimeTable activity);
    Task<List<StudentCourse>> GetAllStudentCoursesByCourse(Course course);
    Task<List<StudentCourse>> GetStudentCourseAndDetailsByEmail(string email);
    Task<List<StudentCourse>> GetAllStudentCourses();
    bool StudentCourseExists(Guid id);
    Task CreateStudentCourse(StudentCourse studentCourse);
    Task CreateStudentCourse(List<StudentCourseViewModel> studentCourse);
    Task UpdateStudentCourse(StudentCourse studentCourse);
    Task DeleteStudentCourse(StudentCourse studentCourse);
    Task<List<Student>> GetAllStudents();
    Task<List<Course>> GetAllCourses();
}