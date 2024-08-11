using AwesomeizeCS.Data;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using AwesomeizeCS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AwesomeizeCS.Repositories
{
    public class TestResultsRepository : ITestResultsRepository
    {
        private readonly ApplicationDbContext _context;

        public TestResultsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ResultsViewModel> ShowResults(Guid? id)
        {
            var codeVersions = await _context.CodeVersion
       .Include(cv => cv.CodeFor)
           .ThenInclude(c => c.Student)
       .Include(cv => cv.CodeFor)
           .ThenInclude(c => c.Assignment)
       .Include(cv => cv.Results)
           .ThenInclude(r => r.Test).FirstOrDefaultAsync(c => c.Id == id);

            return Map(codeVersions);

        }

        public ResultsViewModel Map(CodeVersion codeVersion)
        {
            ResultsViewModel test = new ResultsViewModel
            {
                Exercise = new Assignment
                {
                    Id = codeVersion.CodeFor.Assignment.Id,
                    Order = codeVersion.CodeFor.Assignment.Order,
                    ShortDescription = codeVersion.CodeFor.Assignment.ShortDescription,
                    Content = codeVersion.CodeFor.Assignment.Content,
                    Name = codeVersion.CodeFor.Assignment.Name,
                    VisibleFromWeek = codeVersion.CodeFor.Assignment.VisibleFromWeek,
                    SolvableFromWeek = codeVersion.CodeFor.Assignment.SolvableFromWeek,
                    SolvableToWeek = codeVersion.CodeFor.Assignment.SolvableFromWeek

                },
                TestResults = codeVersion.Results.Where(r => r.Test != null).Select(r => new TestResultViewModel
                {
                    Id = r.Id,
                    Result = r.Result,
                    Output = r.Output,
                    Test = r.Test,
                }).ToList(),
                Errors = codeVersion.Results.Where(r => r.Test == null).Select(r => r.Result).ToList(),
                RunAt = codeVersion.UploadDate
            };

            return test;
        }
    }
}
