using AwesomeizeCS.Data;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using AwesomeizeCS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using TechnologySandbox.Data;

namespace AwesomeizeCS.Repositories;

public class StudentCoursesRepository : IStudentCoursesRepository
{
    private readonly ApplicationDbContext _db;

    public StudentCoursesRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<StudentCourse?> GetStudentCourseById(Guid? id)
    {
        return _db.StudentCourse.Include(sc => sc.Student)
            .ThenInclude(s => s.Assignments).ThenInclude(s => s.Assignment)
            .Include(sc => sc.Course)
            .Include(sc => sc.Attendances)
            .ThenInclude(a => a.Time)
            .Where(sc => sc.Id == id)
            .FirstOrDefaultAsync();
    }

    public Task<List<StudentCourse>> GetAllStudentCoursesByCourse(Course course)
    {
        // get all student courses of a certain course.
        return _db.StudentCourse.Include(sc => sc.Student)
            .ThenInclude(s => s.Assignments)
            .Include(sc => sc.Course)
            .Where(sc => sc.Course == course).ToListAsync();
    }

    public Task<List<StudentCourse>> GetStudentCourseAndDetailsByEmail(string email)
    {
        return _db.StudentCourse
            .Include(s => s.Student)
            .Where(sc => sc.Student.EmailAddress == email)
            .Include(sc => sc.Course)
            .ThenInclude(c => c.Assignments)
            .ThenInclude(a => a.StudentAssignments)
            .Include(sc => sc.Attendances)
            .ThenInclude(a => a.Time)
            .Include(sc => sc.Student)
            .ToListAsync();
    }

    public Task<List<StudentCourse>> GetStudentCoursesByActivity(TimeTable activity)
    {
        var group = activity.For.Split('-')[0];
        bool isGroupActivity = activity.Type == InstructionType.Seminar;

        // placeholder for courses
        if (group == "IE3")
        {
            return _db.StudentCourse
                .Include(s => s.Course)
                .Include(s => s.Student)
                .ThenInclude(s =>
                    s.Assignments.Where(a =>
                        a.Grade == null &&
                        a.Assignment.Type == activity.Type &&
                        activity.Week <= a.Assignment.SolvableToWeek &&
                        a.Assignment.VisibleFromWeek <= activity.Week).OrderBy(a => a.Assignment.Order))
                .ThenInclude(sa => sa.Assignment)
                .ToListAsync();
        }

        // get all student and course data + assignments from a group for an activity
        return _db.StudentCourse
            .Include(s => s.Course)
            .Include(s => s.Student)
            .ThenInclude(s =>
                s.Assignments.Where(a =>
                    a.Grade == null &&
                    a.Assignment.Type == activity.Type &&
                    activity.Week <= a.Assignment.SolvableToWeek &&
                    a.Assignment.VisibleFromWeek <= activity.Week).OrderBy(a => a.Assignment.Order))
            .ThenInclude(sa => sa.Assignment)
            // concrete activity distinction
            .Where(s => isGroupActivity
                ? group == activity.For && s.Course == activity.Course
                : s.AttendingGroup == activity.For && s.Course == activity.Course)
            .ToListAsync();
    }

    public Task<StudentCourse?> GetStudentCourseOrDefault(Guid? id)
    {
        return _db.StudentCourse.Include(sc => sc.Student).Where(sc => sc.Id == id)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public Task<List<StudentCourse>> GetAllStudentCourses()
    {
        return _db.StudentCourse.Include(sc => sc.Student).Include(sc => sc.Course).OrderBy(sc => sc.Student.FirstName)
            .ThenBy(sc => sc.Course.Name).ToListAsync();
    }

    public Task<List<Student>> GetAllStudents()
    {
        return _db.Students.ToListAsync();
    }


    public Task<List<Course>> GetAllCourses()
    {
        return _db.Course.ToListAsync();
    }

    public bool StudentCourseExists(Guid id)
    {
        return _db.StudentCourse.Any(a => a.Id == id);
    }

    public async Task CreateStudentCourse(StudentCourse StudentCourse)
    {
        StudentCourse.Id = Guid.NewGuid();
        _db.Add(StudentCourse);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateStudentCourse(StudentCourse StudentCourse)
    {
        _db.Update(StudentCourse);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteStudentCourse(StudentCourse StudentCourse)
    {
        _db.StudentCourse.Remove(StudentCourse);
        await _db.SaveChangesAsync();
    }

    public ValueTask<Student> GetStudentById(Guid? id)
    {
        return _db.Students.FindAsync(id);
    }

    public ValueTask<Course> GetCourseById(Guid? id)
    {
        return _db.Course.FindAsync(id);
    }

    public async Task CreateStudentCourse(List<StudentCourseViewModel> studentCourse)
    {
        foreach (var studentCourses in studentCourse)
        {
            if (_db.StudentCourse.FirstOrDefaultAsync(sc => sc.Student.EmailAddress.Equals(studentCourses.StudentEmail)
                                                            && sc.Course.Name.Equals(studentCourses.CourseName))
                    .Result == null)
            {
                var NewStudentCourse = new StudentCourse
                {
                    Id = Guid.NewGuid(),
                    AttendingGroup = studentCourses.AttendingGroup,
                    Course = await _db.Course.FirstAsync(c => c.Name.Equals(studentCourses.CourseName)),
                    Student = await _db.Students.FirstAsync(s => s.EmailAddress.Equals(studentCourses.StudentEmail))
                };
                _db.StudentCourse.Add(NewStudentCourse);
            }
        }

        await _db.SaveChangesAsync();
    }
}