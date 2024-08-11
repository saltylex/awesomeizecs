using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using AwesomeizeCS.Repositories.Interfaces;
using AwesomeizeCS.Services.Interfaces;

namespace AwesomeizeCS.Services
{
    public class TestResultsService :ITestResultsService
    {
        private readonly ITestResultsRepository _repository;
        public TestResultsService(ITestResultsRepository repository)
        {
            _repository = repository;
        }
        public ResultsViewModel Map(CodeVersion codeVersion)
        {
            return _repository.Map(codeVersion);
        }

        public async Task<ResultsViewModel> ShowResults(Guid? id)
        {
            return await _repository.ShowResults(id);
        }
    }
}
