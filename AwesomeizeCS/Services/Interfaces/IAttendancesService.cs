using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;

namespace AwesomeizeCS.Services.Interfaces
{
    public interface IAttendancesService
    {
        bool AttendanceExists(Guid id);
        bool AttendanceExists(StudentCourse studentCourse, TimeTable time);
        Task CreateAttendance(Attendance attendance);
        Task CreateAttendance(AttendanceViewModel attendance);
        Task DeleteAttendance(Attendance attendance);
        Task<List<Attendance>> GetAllAttendances();
        Task<List<AttendanceDataViewModel>> GetAttendanceForValidation();
        Task<List<Attendance>> GetAttendanceForCurrentActivityForValidation(TimeTable activity);
        Task<List<StudentCourse>> GetAllStudentCourses();
        Task<List<TimeTable>> GetAllTimeTables();
        ValueTask<Attendance?> GetAttendanceById(Guid? id);
        Task<Attendance?> GetAttendanceOrDefault(Guid? id);
        Task<List<Attendance>> GetAttendancesForCourse(string courseName, string userEmail);
        Task UpdateAttendance(Attendance attendance);
        Task<AttendanceViewModel?> GetAttendanceFormData( string userEmail);
        Task UpdateAttendanceValidation(Guid id, bool isValidated);
    }
}