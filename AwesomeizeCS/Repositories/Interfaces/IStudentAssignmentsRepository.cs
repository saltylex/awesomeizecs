using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;

namespace AwesomeizeCS.Repositories.Interfaces;

public interface IStudentAssignmentsRepository
{
    Task<StudentAssignment?> GetStudentAssignmentById(Guid? id);
    Task<StudentAssignment?> GetStudentAssignmentOrDefault(Guid? id);
    Task<List<StudentAssignment>> GetAllStudentAssignments();
    bool StudentAssignmentExists(Guid id);
    Task CreateStudentAssignment(StudentAssignment studentAssignment);
    Task UpdateStudentAssignment(StudentAssignment studentAssignment);
    Task DeleteStudentAssignment(StudentAssignment studentAssignment);
    ValueTask<Student?> GetStudentById(Guid id);
    ValueTask<Assignment?> GetAssignmentById(Guid id);
    Task<List<Student>> GetAllStudents();
    Task<List<Assignment>> GetAllAssignments();
    Task UpdateStudentAssignmentGrade(Guid assignmentId, decimal newValue);
    Task UpdateStudentAssignmentBonus(Guid assignmentId, decimal newValue);
    Task<List<StudentAssignmentGradingViewModel>> GetStudentAssignmentsForGrading();
    Task UpdateStudentAssignmentGrade(Guid id, decimal? grade, decimal? bonus);
    public Task RunTests(string filePath, Guid assignmentId, Guid studentId, Guid codeVersionId);
}