using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;

namespace AwesomeizeCS.Services.Interfaces
{
    public interface ICoursesService
    {
        bool CourseExists(Guid id);
        Task CreateCourse(Course course);
        Task DeleteCourse(Course course);
        Task<List<CoursesTeacherNameViewModel>> GetAllCourses();
        Task<List<Course>> GetCoursesByMainTeacher(Guid id);
        ValueTask<Course?> GetCourseById(Guid? id);
        Task<Course?> GetCourseOrDefault(Guid? id);
        Task<List<StudentSituationViewModel>> GetStudentSituationData();
        Task UpdateCourse(Course course);
    }
}