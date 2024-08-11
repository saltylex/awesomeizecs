using AwesomeizeCS.Data;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using AwesomeizeCS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AwesomeizeCS.Repositories;

public class AttendancesRepository : IAttendancesRepository
{
    private readonly ApplicationDbContext _db;

    public AttendancesRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public ValueTask<Attendance?> GetAttendanceById(Guid? id)
    {
        return _db.Attendance.FindAsync(id);
    }

    public Task<Attendance?> GetAttendanceOrDefault(Guid? id)
    {
        return _db.Attendance.FirstOrDefaultAsync(b => b.Id == id);
    }

    public Task<List<Attendance>> GetAllAttendances()
    {
        return _db.Attendance.Include(a => a.Time).Include(a => a.StudentCourse).ToListAsync();
    }

    public bool AttendanceExists(Guid id)
    {
        return _db.Attendance.Any(a => a.Id == id);
    }

    public async Task CreateAttendance(Attendance attendance)
    {
        attendance.Id = Guid.NewGuid();
        _db.Add(attendance);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAttendance(Attendance attendance)
    {
        _db.Update(attendance);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAttendance(Attendance attendance)
    {
        _db.Attendance.Remove(attendance);
        await _db.SaveChangesAsync();
    }

    public Task<List<StudentCourse>> GetAllStudentCourses()
    {
        return _db.StudentCourse.Include(sc => sc.Student).Include(sc => sc.Course).ToListAsync();
    }

    public Task<List<TimeTable>> GetAllTimeTables()
    {
        return _db.TimeTable.Include(t => t.Course).ToListAsync();
    }

    public ValueTask<TimeTable?> GetTimeTableById(Guid? id)
    {
        return _db.TimeTable.FindAsync(id);
    }

    public Task<StudentCourse?> GetStudentCourseById(Guid? id)
    {
        return _db.StudentCourse.Include(sc => sc.Student).Include(sc => sc.Course).Where(sc => sc.Id == id)
            .FirstOrDefaultAsync();
    }

    public ValueTask<Student?> GetStudentById(Guid? id)
    {
        return _db.Students.FindAsync(id);
    }

    public ValueTask<Course?> GetCourseById(Guid? id)
    {
        return _db.Course.FindAsync(id);
    }

    public async Task<List<Attendance>> GetAttendancesForCourse(string courseName, string userEmail)
    {
        var student = await _db.Students.FirstOrDefaultAsync(s => s.EmailAddress == userEmail);
        if (student == null)
        {
            return null;
        }

        var course = await _db.Course.FirstOrDefaultAsync(c => c.Name == courseName);
        if (course == null)
        {
            return null;
        }

        var studentCourse = await _db.StudentCourse
            .Include(sc => sc.Attendances)
            .FirstOrDefaultAsync(sc => sc.Student.Id == student.Id && sc.Course.Id == course.Id);

        if (studentCourse == null)
        {
            return null;
        }

        //var studentCourse = _db.StudentCourse.Include(sc => sc.Student).Include(sc => sc.Course).Where(sc => sc.Student.Id == student.Id && sc.Course.Equals(course)).FirstOrDefaultAsync();
        var attendance = await _db.Attendance.Include(at => at.StudentCourse).Include(at => at.Time)
            .Where(at => at.StudentCourse.Id == studentCourse.Id).ToListAsync();

        return attendance;
    }

    public async Task<AttendanceViewModel?> GetAttendanceFormData(string userEmail)
    {
        var student = await _db.Students.FirstOrDefaultAsync(s => s.EmailAddress == userEmail);
        if (student == null)
        {
            return null;
        }

        
        DateTime now = DateTime.Now;

        var attendanceData = await (from sc in _db.StudentCourse
                join s in _db.Students on sc.Student.Id equals s.Id
                join t in _db.TimeTable on sc.Course.Id equals t.Course.Id
                where s.Id == student.Id 
                      && (sc.AttendingGroup == t.For || t.For == "IE3" || sc.AttendingGroup.Contains(t.For))
                      && t.StartsAt <= now 
                      && t.EndsAt >= now
                select new AttendanceViewModel
                {
                    Id = Guid.NewGuid(),
                    StudentCourse = sc,
                    Time = t
                }
            ).FirstOrDefaultAsync();

        return attendanceData;
    }

    public bool AttendanceExists(StudentCourse studentCourse, TimeTable time)
    {
        return _db.Attendance.Any(a => a.StudentCourse == studentCourse && a.Time == time);
    }

    public async Task UpdateAttendanceValidation(Guid attendanceId, bool isValidated)
    {
        var attendanceRecord = await _db.Attendance.FindAsync(attendanceId);
        if (attendanceRecord != null)
        {
            attendanceRecord.IsValidated = isValidated;

            await _db.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentException($"Attendance record with ID {attendanceId} not found.");
        }
    }

    public async Task<List<AttendanceDataViewModel>> GetAttendanceForValidation()
    {
        var attendancesData = await (from a in _db.Attendance
            join sc in _db.StudentCourse on a.StudentCourse.Id equals sc.Id
            join s in _db.Students on a.StudentCourse.Student.Id equals s.Id
            join t in _db.TimeTable on a.Time.Id equals t.Id
            join c in _db.Course on a.StudentCourse.Course.Id equals c.Id
            select new AttendanceDataViewModel
            {
                Id = a.Id,
                StudentCourse = sc,
                Time = t,
                CourseName = c.Name,
                StudentName = s.FirstName + " " + s.LastName,
                IsValidated = true,
                StartsAt = t.StartsAt,
                EndsAt = t.EndsAt
            }).ToListAsync();
        return attendancesData;
    }

    public Task<List<Attendance>> GetAttendanceForCurrentActivityForValidation(TimeTable activity)
    {
        return _db.Attendance.Include(a => a.StudentCourse)
            .Include(a => a.Time)
            .Where(a => a.Time == activity)
            .ToListAsync();
    }
}