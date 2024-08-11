using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using AwesomeizeCS.Repositories;
using AwesomeizeCS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AwesomeizeCS.Services;

public class TeacherCoursesService : ITeacherCoursesService
{
    private readonly ITeacherCoursesRepository _repository;

    public TeacherCoursesService(ITeacherCoursesRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Course>> GetAllCourses()
    {
        return _repository.GetAllCourses();
    }

    public ValueTask<Course?> GetCourseById(Guid? id)
    {
        return _repository.GetCourseById(id);
    }

    public ValueTask<TeacherCourse?> GetTeacherCourseById(Guid? id)
    {
        return _repository.GetTeacherCourseById(id);
    }

    public Task<TeacherCourse?> GetTeacherCourseOrDefault(Guid? id)
    {
        return _repository.GetTeacherCourseOrDefault(id);
    }

    public Task<List<TeacherCourse>> GetAllTeachersByCourse(Course course)
    {
        return _repository.GetAllTeachersByCourse(course);
    }

    public Task<string?> GetTeacherNameById(Guid id)
    {
        return _repository.GetTeacherNameById(id);
    }

    public Task<List<IdentityUser>> GetAllTeachers()
    {
        return _repository.GetAllTeachers();
    }

    public Task<List<TeacherCourseViewModel>> GetAllTeacherCourses()
    {
        return _repository.GetAllTeacherCourses();
    }

    public async Task CreateTeacherCourse(TeacherCourse teacherCourse)
    {
        teacherCourse.Course = await _repository.GetCourseById(teacherCourse.Course.Id);
        await _repository.CreateTeacherCourse(teacherCourse);
    }

    public async Task DeleteTeacherCourse(TeacherCourse teacherCourse)
    {
        await _repository.DeleteTeacherCourse(teacherCourse);
    }

    public async Task UpdateTeacherCourse(TeacherCourse teacherCourse)
    {
        await _repository.UpdateTeacherCourse(teacherCourse);
    }

    public bool TeacherCourseExists(Guid id)
    {
        return _repository.TeacherCourseExists(id);
    }
}
