using AwesomeizeCS.Domain;

namespace AwesomeizeCS.Repositories.Interfaces;

public interface IAssignmentsRepository
{
    ValueTask<Assignment?> GetAssignmentById(Guid? id);
    Task<Assignment?> GetAssignmentOrDefault(Guid? id);
    Task<List<Assignment>> GetAllAssignments();
    Task<List<Assignment>?> GetChildrenAssignments(Assignment assignment);
    Task<List<Assignment>> GetAllSubproblems(Guid id);
    Task<List<Course>> GetAllCourses();
    bool AssignmentExists(Guid id);
    Task CreateAssignment(Assignment assignment);
    Task UpdateAssignment(Assignment assignment);
    Task DeleteAssignment(Assignment assignment);
    ValueTask<Course?> GetCourseById(Guid courseId);
}