using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using Microsoft.AspNetCore.Identity;

namespace AwesomeizeCS.Repositories;

public interface ITeacherCoursesRepository
{
    ValueTask<TeacherCourse?> GetTeacherCourseById(Guid? id);
    Task<TeacherCourse?> GetTeacherCourseOrDefault(Guid? id);
    Task<List<TeacherCourse>> GetAllTeachersByCourse(Course course);
    Task<string?> GetTeacherNameById(Guid id);
    Task<List<IdentityUser>> GetAllTeachers();
    Task<List<TeacherCourseViewModel>> GetAllTeacherCourses();
    Task<List<Course>> GetAllCourses();
    ValueTask<Course?> GetCourseById(Guid? id);
    Task CreateTeacherCourse(TeacherCourse teacherCourse);
    Task DeleteTeacherCourse(TeacherCourse teacherCourse);
    Task UpdateTeacherCourse(TeacherCourse teacherCourse);
    bool TeacherCourseExists(Guid id);
}