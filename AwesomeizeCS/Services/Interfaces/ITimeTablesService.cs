using AwesomeizeCS.Domain;
using System.Threading.Tasks;

namespace AwesomeizeCS.Services.Interfaces
{
    public interface ITimeTablesService
    {
        Task CreateTimeTable(TimeTable timeTable);
        Task DeleteTimeTable(TimeTable timeTable);
        Task<List<TimeTable>> GetTimetablesForTeacher(Guid teacherId);
        Task<TimeTable?> GetCurrentActivityTimeTable(DateTime time, string teacherId);
        Task<List<Course>> GetAllCourses();
        Task<List<TimeTable>> GetAllTimeTables();
        ValueTask<TimeTable?> GetTimeTableById(Guid? id);
        Task<TimeTable?> GetTimeTableOrDefault(Guid? id);
        bool TimeTableExists(Guid id);
        Task UpdateTimeTable(TimeTable timeTable);
    }
}