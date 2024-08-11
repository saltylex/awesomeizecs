using AwesomeizeCS.Data;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AwesomeizeCS.Repositories;

public class TimeTablesRepository : ITimeTablesRepository
{
    private readonly ApplicationDbContext _db;

    public TimeTablesRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<List<TimeTable>> GetTimetablesForTeacher(Guid teacherId)
    {
        return _db.TimeTable.Include(t=>t.Course).Where(t => t.TaughtBy == teacherId && t.StartsAt.Date == DateTime.Today)
            .OrderBy(t => t.StartsAt).ToListAsync();
    }

    public Task<TimeTable?> GetCurrentActivityTimeTable(DateTime time, string teacherId)
    {
        return _db.TimeTable.Include(t => t.Course).Where(t => t.TaughtBy == new Guid(teacherId)).Where(t =>
            (DateTime.Compare(t.StartsAt, time) == 0 || DateTime.Compare(t.StartsAt, time) < 0) &&
            (DateTime.Compare(time, t.EndsAt) == 0 || DateTime.Compare(time, t.EndsAt) < 0)).FirstOrDefaultAsync();
    }

    public ValueTask<TimeTable?> GetTimeTableById(Guid? id)
    {
        return _db.TimeTable.FindAsync(id);
    }

    public Task<TimeTable?> GetTimeTableOrDefault(Guid? id)
    {
        return _db.TimeTable.FirstOrDefaultAsync(b => b.Id == id);
    }

    public Task<List<TimeTable>> GetAllTimeTables()
    {
        return _db.TimeTable.Include(t => t.Course).OrderBy(t => t.Course).ThenBy(t => t.For).ThenBy(t => t.Week)
            .ToListAsync();
    }

    public bool TimeTableExists(Guid id)
    {
        return _db.TimeTable.Any(a => a.Id == id);
    }

    public async Task CreateTimeTable(TimeTable TimeTable)
    {
        TimeTable.Id = Guid.NewGuid();
        _db.Add(TimeTable);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateTimeTable(TimeTable TimeTable)
    {
        _db.Update(TimeTable);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteTimeTable(TimeTable TimeTable)
    {
        _db.TimeTable.Remove(TimeTable);
        await _db.SaveChangesAsync();
    }

    public Task<List<Course>> GetAllCourses()
    {
        return _db.Course.ToListAsync();
    }

    public ValueTask<Course?> GetCourseById(Guid id)
    {
        return _db.Course.FindAsync(id);
    }
}