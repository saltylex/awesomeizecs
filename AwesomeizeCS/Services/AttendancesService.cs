using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using AwesomeizeCS.Repositories.Interfaces;
using AwesomeizeCS.Services.Interfaces;

namespace AwesomeizeCS.Services
{
    public class AttendancesService : IAttendancesService
    {
        private readonly IAttendancesRepository _repository;
        public AttendancesService(IAttendancesRepository repository)
        {
            _repository = repository;
        }

        public ValueTask<Attendance?> GetAttendanceById(Guid? id)
        {
            return _repository.GetAttendanceById(id);
        }
        public Task<Attendance?> GetAttendanceOrDefault(Guid? id)
        {
            return _repository.GetAttendanceOrDefault(id);
        }

        public Task<List<Attendance>> GetAllAttendances()
        {
            return _repository.GetAllAttendances();
        }

        public bool AttendanceExists(Guid id)
        {
            return _repository.AttendanceExists(id);
        }

        public bool AttendanceExists(StudentCourse studentCourse, TimeTable time)
        {
            return _repository.AttendanceExists(studentCourse, time);
        }

        public async Task CreateAttendance(Attendance attendance)
        {
            attendance.StudentCourse = await _repository.GetStudentCourseById(attendance.StudentCourse.Id);
            attendance.Time = await _repository.GetTimeTableById(attendance.Time.Id);
            attendance.StudentCourse.Student = await _repository.GetStudentById(attendance.StudentCourse.Student.Id);
            attendance.StudentCourse.Course = await _repository.GetCourseById(attendance.StudentCourse.Course.Id);
            await _repository.CreateAttendance(attendance);
        }

        public async Task UpdateAttendance(Attendance attendance)
        {
            await _repository.UpdateAttendance(attendance);
        }

        public async Task DeleteAttendance(Attendance attendance)
        {
            await _repository.DeleteAttendance(attendance);
        }

        public Task<List<Attendance>> GetAttendanceForCurrentActivityForValidation(TimeTable activity)
        {
            return _repository.GetAttendanceForCurrentActivityForValidation(activity);
        }

        public Task<List<StudentCourse>> GetAllStudentCourses()
        {
            return _repository.GetAllStudentCourses();
        }

        public Task<List<TimeTable>> GetAllTimeTables()
        {
            return _repository.GetAllTimeTables();
        }

        public Task<List<Attendance>> GetAttendancesForCourse(string courseName, string userEmail)
        {
            return _repository.GetAttendancesForCourse(courseName, userEmail);
        }


        public Task<AttendanceViewModel?> GetAttendanceFormData( string userEmail)
        {
            return _repository.GetAttendanceFormData( userEmail);
        }

        public Task CreateAttendance(AttendanceViewModel attendance)
        {
            Attendance newAttendance = new Attendance();
            newAttendance.StudentCourse=attendance.StudentCourse;
            newAttendance.Time=attendance.Time;
            return CreateAttendance(newAttendance);
        }

        public async Task UpdateAttendanceValidation(Guid id, bool isValidated)
        {
            await _repository.UpdateAttendanceValidation(id, isValidated);
            
        }

        public Task<List<AttendanceDataViewModel>> GetAttendanceForValidation()
        {
            return _repository.GetAttendanceForValidation();
        }
    }
}

