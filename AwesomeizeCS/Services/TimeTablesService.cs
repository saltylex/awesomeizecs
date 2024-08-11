using AwesomeizeCS.Domain;
using AwesomeizeCS.Repositories.Interfaces;
using AwesomeizeCS.Services.Interfaces;
using System.Text.RegularExpressions;

namespace AwesomeizeCS.Services
{
    public class TimeTablesService : ITimeTablesService
    {
        private readonly ITimeTablesRepository _repository;
        public TimeTablesService(ITimeTablesRepository repository)
        {
            _repository = repository;
        }

        public ValueTask<TimeTable?> GetTimeTableById(Guid? id)
        {
            return _repository.GetTimeTableById(id);
        }

        public Task<List<TimeTable>> GetTimetablesForTeacher(Guid teacherId)
        {
            return _repository.GetTimetablesForTeacher(teacherId);
        }

        public Task<TimeTable?> GetCurrentActivityTimeTable(DateTime time, string teacherId)
        {
            return _repository.GetCurrentActivityTimeTable(time, teacherId);
        }
        public Task<TimeTable?> GetTimeTableOrDefault(Guid? id)
        {
            return _repository.GetTimeTableOrDefault(id);
        }

        public Task<List<TimeTable>> GetAllTimeTables()
        {
            return _repository.GetAllTimeTables();
        }

        public bool TimeTableExists(Guid id)
        {
            return _repository.TimeTableExists(id);
        }

        public async Task CreateTimeTable(TimeTable timeTable)
        {
            timeTable.Course = await _repository.GetCourseById(timeTable.Course.Id);
            ValidateTimeTable(timeTable);
            await _repository.CreateTimeTable(timeTable);
        }

        public async Task UpdateTimeTable(TimeTable timeTable)
        {
            ValidateTimeTable(timeTable);
            await _repository.UpdateTimeTable(timeTable);
        }

        public async Task DeleteTimeTable(TimeTable timeTable)
        {
            await _repository.DeleteTimeTable(timeTable);
        }

        private void ValidateTimeTable(TimeTable timeTable)
        {
            var pattern = @"^[0-9]{3}-[12]$";
            var patternGroup = @"^[0-9]{3}$";
            if (!(Regex.IsMatch(timeTable.For, pattern) || Regex.IsMatch(timeTable.For, patternGroup) || timeTable.For == "everyone" || timeTable.For == "IE3"))
            {
                var fieldName = nameof(timeTable.For);
                    var errorMessage = "invalid Group";
                    throw new ArgumentException($"Field: {fieldName}, Error: {errorMessage}");
                }

            bool isValidYear = int.TryParse(timeTable.AcademicYear, out int year) &&
                               year >= 2023 &&
                               year <= 2100;
            if (!isValidYear)
            {
                var fieldName = nameof(timeTable.AcademicYear);
                var errorMessage = "The year is invalid.";
                throw new ArgumentException($"Field: {fieldName}, Error: {errorMessage}");
            }


            if (timeTable.EndsAt < timeTable.StartsAt)
            {
                var fieldName = nameof(timeTable.StartsAt);
                var errorMessage = "End time must be after Start time.";
                throw new ArgumentException($"Field: {fieldName}, Error: {errorMessage}");

            }
        }
        public Task<List<Course>> GetAllCourses()
        {
            return _repository.GetAllCourses();
        }
    }
}
