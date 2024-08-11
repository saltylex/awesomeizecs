using AwesomeizeCS.Domain;

namespace AwesomeizeCS.Repositories.Interfaces;

public interface ITimeTablesRepository
{
    ValueTask<TimeTable?> GetTimeTableById(Guid? id);
    Task<List<TimeTable>> GetTimetablesForTeacher(Guid teacherId);
    Task<TimeTable?> GetCurrentActivityTimeTable(DateTime time, string teacherId);
    Task<TimeTable?> GetTimeTableOrDefault(Guid? id);
    Task<List<TimeTable>> GetAllTimeTables();
    bool TimeTableExists(Guid id);
    Task CreateTimeTable(TimeTable TimeTable);
    Task UpdateTimeTable(TimeTable TimeTable);
    Task DeleteTimeTable(TimeTable TimeTable);
    Task<List<Course>> GetAllCourses();
    ValueTask<Course?> GetCourseById(Guid id);
}