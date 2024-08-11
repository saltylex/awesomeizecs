using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;

namespace AwesomeizeCS.Repositories.Interfaces;

public interface IAttendancesRepository
{
    ValueTask<Attendance?> GetAttendanceById(Guid? id);
    Task<Attendance?> GetAttendanceOrDefault(Guid? id);
    Task<List<Attendance>> GetAllAttendances();
    Task<List<Attendance>> GetAttendanceForCurrentActivityForValidation(TimeTable activity);
    bool AttendanceExists(Guid id);
    Task CreateAttendance(Attendance attendance);
    Task UpdateAttendance(Attendance attendance);
    Task DeleteAttendance(Attendance attendance);
    Task<List<StudentCourse>> GetAllStudentCourses();
    Task<List<TimeTable>> GetAllTimeTables();
    public ValueTask<TimeTable?> GetTimeTableById(Guid? id);
    public Task<StudentCourse?> GetStudentCourseById(Guid? id);
    public ValueTask<Student?> GetStudentById(Guid? id);
    public ValueTask<Course?> GetCourseById(Guid? id);
    Task<List<Attendance>> GetAttendancesForCourse(string courseName, string userEmail);
    Task<AttendanceViewModel?> GetAttendanceFormData(string userEmail);
    bool AttendanceExists(StudentCourse studentCourse, TimeTable time);
    Task UpdateAttendanceValidation(Guid attendanceId, bool isValidated);
    Task<List<AttendanceDataViewModel>> GetAttendanceForValidation();
}