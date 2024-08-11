using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;

namespace AwesomeizeCS.Repositories.Interfaces;

public interface ICoursesRepository
{
    ValueTask<Course?> GetCourseById(Guid? id);
    Task<Course?> GetCourseOrDefault(Guid? id);
    Task<List<CoursesTeacherNameViewModel>> GetAllCourses();
    Task<List<Course>> GetCoursesByMainTeacher(Guid id);
    bool CourseExists(Guid id);
    Task CreateCourse(Course course);
    Task UpdateCourse(Course course);
    Task DeleteCourse(Course course);
    Task<List<StudentSituationViewModel>> GetStudentSituationData();
}