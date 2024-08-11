using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;

namespace AwesomeizeCS.Services.Interfaces
{
    public interface ITestResultsService
    {
        public ResultsViewModel Map(CodeVersion codeVersion);
        Task<ResultsViewModel> ShowResults(Guid? id);
    }
}
