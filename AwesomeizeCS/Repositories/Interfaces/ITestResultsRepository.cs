using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;

namespace AwesomeizeCS.Repositories.Interfaces
{
    public interface ITestResultsRepository
    {
        public ResultsViewModel Map(CodeVersion codeVersion);
        Task<ResultsViewModel> ShowResults(Guid? id);
    }
}
