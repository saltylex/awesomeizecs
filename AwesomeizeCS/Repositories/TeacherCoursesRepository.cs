using AwesomeizeCS.Data;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AwesomeizeCS.Repositories;

public class TeacherCoursesRepository : ITeacherCoursesRepository
{
    private readonly ApplicationDbContext _db;

    public TeacherCoursesRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public ValueTask<TeacherCourse?> GetTeacherCourseById(Guid? id)
    {
        return _db.TeacherCourse.FindAsync(id);
    }

    public Task<TeacherCourse?> GetTeacherCourseOrDefault(Guid? id)
    {
        return _db.TeacherCourse.FirstOrDefaultAsync(b => b.Id == id);
    }

    public Task<List<TeacherCourse>> GetAllTeachersByCourse(Course course)
    {
        return _db.TeacherCourse.Where(tc => tc.Course.Id == course.Id).ToListAsync();
    }

    public ValueTask<Course?> GetCourseById(Guid id)
    {
        return _db.Course.FindAsync(id);
    }

    public Task<string?> GetTeacherNameById(Guid id)
    {
        return _db.Users.Where(u => u.Id == id.ToString()).Select(u => u.UserName).FirstOrDefaultAsync();
    }

    public async Task<List<IdentityUser>> GetAllTeachers()
    {
        var teacherRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Teacher");
        if (teacherRole == null)
        {
            return new List<IdentityUser>();
        }

        var teachersIds = await _db.UserRoles.Where(u => u.RoleId == teacherRole.Id).Select(u => u.UserId).ToListAsync();
        var teachers = await _db.Users.Where(u => teachersIds.Contains(u.Id)).ToListAsync();

        return teachers;
    }

    public async Task<List<TeacherCourseViewModel>> GetAllTeacherCourses()
    {

        var TCviewmodel = await (from tc in _db.TeacherCourse
                           join user in _db.Users on tc.TeacherId.ToString() equals user.Id
                           join course in _db.Course on tc.Course.Id equals course.Id
                           select new TeacherCourseViewModel
                           {
                               Id =tc.Id,
                               TeacherId = tc.Id,
                               Course = tc.Course,
                               IsMainTeacher = tc.IsMainTeacher,
                               Username = user.UserName
                           }).ToListAsync();

        return TCviewmodel;
        //var TCviewmodel = _db.TeacherCourse.Include(tc => tc.Course).ToListAsync();


    }

    public Task<List<Course>> GetAllCourses()
    {
        return _db.Course.ToListAsync();
    }

    public ValueTask<Course?> GetCourseById(Guid? id)
    {
        return _db.Course.FindAsync(id);
    }

    public async Task CreateTeacherCourse(TeacherCourse teacherCourse)
    {
        teacherCourse.Id = Guid.NewGuid();
        _db.Add(teacherCourse);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteTeacherCourse(TeacherCourse teacherCourse)
    {
        _db.TeacherCourse.Remove(teacherCourse);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateTeacherCourse(TeacherCourse teacherCourse)
    {
        _db.Update(teacherCourse);
        await _db.SaveChangesAsync();
    }

    public bool TeacherCourseExists(Guid id)
    {
        return _db.TeacherCourse.Any(a => a.Id == id);
    }
}