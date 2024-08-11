using AwesomeizeCS.Domain;

namespace AwesomeizeCS.Services.Interfaces
{
    public interface IAssignmentsService
    {
        bool AssignmentExists(Guid id);
        Task CreateAssignment(Assignment assignment);
        Task DeleteAssignment(Assignment assignment);
        Task<List<Assignment>> GetAllAssignments();
        Task<List<Assignment>?> GetChildrenAssignments(Assignment assignment);
        Task<List<Assignment>> GetAllSubproblems(Guid id);
        Task<List<Course>> GetAllCourses();
        ValueTask<Assignment?> GetAssignmentById(Guid? id);
        Task<Assignment?> GetAssignmentOrDefault(Guid? id);
        Task UpdateAssignment(Assignment assignment);
    }
}