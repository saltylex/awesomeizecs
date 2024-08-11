using AwesomeizeCS.Domain;
using AwesomeizeCS.Repositories.Interfaces;
using AwesomeizeCS.Services.Interfaces;

namespace AwesomeizeCS.Services
{
    public class AssignmentsService : IAssignmentsService
    {
        private readonly IAssignmentsRepository _repository;

        public AssignmentsService(IAssignmentsRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Assignment>?> GetChildrenAssignments(Assignment assignment)
        {
            return _repository.GetChildrenAssignments(assignment);
        }

        public Task<List<Assignment>> GetAllSubproblems(Guid id)
        {
            return _repository.GetAllSubproblems(id);
        }

        public Task<List<Course>> GetAllCourses()
        {
            return _repository.GetAllCourses();
        }

        public ValueTask<Assignment?> GetAssignmentById(Guid? id)
        {
            return _repository.GetAssignmentById(id);
        }

        public Task<Assignment?> GetAssignmentOrDefault(Guid? id)
        {
            return _repository.GetAssignmentOrDefault(id);
        }

        public Task<List<Assignment>> GetAllAssignments()
        {
            return _repository.GetAllAssignments();
        }

        public bool AssignmentExists(Guid id)
        {
            return _repository.AssignmentExists(id);
        }

        public async Task CreateAssignment(Assignment assignment)
        {
            
            assignment.Course = await _repository.GetCourseById(assignment.Course.Id);
            assignment.Parent = await _repository.GetAssignmentById(assignment.Parent?.Id) ?? null;

            await _repository.CreateAssignment(assignment);
        }

        public async Task UpdateAssignment(Assignment assignment)
        {
            await _repository.UpdateAssignment(assignment);
        }

        public async Task DeleteAssignment(Assignment assignment)
        {
            await _repository.DeleteAssignment(assignment);
        }
    }
}