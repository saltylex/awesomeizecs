using AwesomeizeCS.Domain;

namespace AwesomeizeCS.Services.Interfaces
{
    public interface IIOTestsService
    {
        Task<IEnumerable<IOTest>> GetAllTestsAsync();
        Task<IOTest> GetTestByIdAsync(Guid id);
        Task<IOTest> GetTestByIdWithStepsAsync(Guid id);
        Task CreateTestAsync(Guid? assignmentId,IOTest test);
        Task UpdateTestAsync(IOTest test);
        Task DeleteTestAsync(Guid id);
        Task AddStepToTestAsync(Guid testId, TestStep step);
        Task DeleteStepFromTestAsync(Guid testId, Guid stepId);
        Task MoveStepUpAsync(Guid testId, Guid stepId);
        Task MoveStepDownAsync(Guid testId, Guid stepId);
        Task TextToStepView(Guid testId, string step);
        bool IOTestExists(Guid id);
    }
}
