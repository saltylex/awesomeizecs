using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;

namespace AwesomeizeCS.Services.Interfaces
{
    public interface IStudentAssignmentsService
    {
        Task CreateStudentAssignment(StudentAssignment studentAssignment);
        Task DeleteStudentAssignment(StudentAssignment studentAssignment);
        Task<List<StudentAssignment>> GetAllStudentAssignments();
        Task<List<Student>> GetAllStudents();
        Task<List<Assignment>> GetAllAssignments();
        Task<StudentAssignment?> GetStudentAssignmentById(Guid? id);
        Task<StudentAssignment?> GetStudentAssignmentOrDefault(Guid? id);
        bool StudentAssignmentExists(Guid id);
        Task UpdateStudentAssignment(StudentAssignment studentAssignment);
        Task UpdateStudentAssignmentGrade(Guid assignmentId, decimal newValue);
        Task UpdateStudentAssignmentBonus(Guid assignmentId, decimal newValue);
        Task<List<StudentAssignmentGradingViewModel>> GetStudentAssignmentsForGrading();
        Task UpdateStudentAssignmentGrade(Guid id, decimal? grade, decimal? bonus );
        public Task RunTests(string filePath, Guid assignmentId, Guid studentId, Guid codeVersionId);
    }
}