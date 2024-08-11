using AwesomeizeCS.Domain;
using AwesomeizeCS.Repositories;
using AwesomeizeCS.Repositories.Interfaces;
using AwesomeizeCS.Services.Interfaces;

namespace AwesomeizeCS.Services
{
    public class IOTestsService : IIOTestsService
    {
        private readonly IIOTestsRepository _repository;

        public IOTestsService(IIOTestsRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<IOTest>> GetAllTestsAsync()
        {
            return _repository.GetAllTestsAsync();
        }

        public  Task<IOTest> GetTestByIdWithStepsAsync(Guid id)
        {
            return  _repository.GetTestByIdWithStepsAsync(id);
        }

        public Task<IOTest> GetTestByIdAsync(Guid id)
        {
            return _repository.GetTestByIdAsync(id);
        }

        public Task CreateTestAsync(Guid? assignmentId, IOTest test)
        {
            return _repository.CreateTestAsync(assignmentId, test);
        }

        public Task UpdateTestAsync(IOTest test)
        {
            return _repository.UpdateTestAsync(test);
        }

        public Task DeleteTestAsync(Guid id)
        {
            return _repository.DeleteTestAsync(id);
        }

        public Task AddStepToTestAsync(Guid testId, TestStep step)
        {
            return _repository.AddStepToTestAsync(testId, step);
        }

        public Task DeleteStepFromTestAsync(Guid testId, Guid stepId)
        {
            return _repository.DeleteStepFromTestAsync(testId, stepId);
        }

        public Task MoveStepUpAsync(Guid testId, Guid stepId)
        {
            return _repository.MoveStepUpAsync(testId, stepId);
        }

        public Task MoveStepDownAsync(Guid testId, Guid stepId)
        {
            return _repository.MoveStepDownAsync(testId, stepId);
        }

        public Task TextToStepView(Guid testId, string step)
        {
            return _repository.TextToStepView(testId, step);
        }
        public bool IOTestExists(Guid id)
        {
            return _repository.IOTestExists( id);
        }
    }
}

