using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using AwesomeizeCS.Repositories.Interfaces;
using AwesomeizeCS.Services.Interfaces;
using System.Diagnostics;

namespace AwesomeizeCS.Services
{
    public class StudentAssignmentsService : IStudentAssignmentsService
    {
        private readonly IStudentAssignmentsRepository _repository;
        public StudentAssignmentsService(IStudentAssignmentsRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Assignment>> GetAllAssignments()
        {
            return _repository.GetAllAssignments();
        }

        public Task<StudentAssignment?> GetStudentAssignmentById(Guid? id)
        {
            return _repository.GetStudentAssignmentById(id);
        }
        public Task<StudentAssignment?> GetStudentAssignmentOrDefault(Guid? id)
        {
            return _repository.GetStudentAssignmentOrDefault(id);
        }

        public Task<List<StudentAssignment>> GetAllStudentAssignments()
        {
            return _repository.GetAllStudentAssignments();
        }

        public bool StudentAssignmentExists(Guid id)
        {
            return _repository.StudentAssignmentExists(id);
        }

        public async Task CreateStudentAssignment(StudentAssignment studentAssignment)
        {
            studentAssignment.Student = await _repository.GetStudentById(studentAssignment.Student.Id);
            studentAssignment.Assignment = await _repository.GetAssignmentById(studentAssignment.Assignment.Id);
            ValidateStudentAssignment(studentAssignment);

            await _repository.CreateStudentAssignment(studentAssignment);
        }

        public async Task UpdateStudentAssignment(StudentAssignment studentAssignment)
        {
            ValidateStudentAssignment(studentAssignment);

            await _repository.UpdateStudentAssignment(studentAssignment);
        }

        public async Task DeleteStudentAssignment(StudentAssignment studentAssignment)
        {
            await _repository.DeleteStudentAssignment(studentAssignment);
        }

        private void ValidateStudentAssignment(StudentAssignment studentAssignment)
        {
            if (studentAssignment.Grade < 0)
            {
                var fieldName = nameof(studentAssignment.Grade);
                var errorMessage = "Grade cannot be negative";
                throw new ArgumentException($"Field: {fieldName}, Error: {errorMessage}");
            }

            if (studentAssignment.Grade > 10)
            {
                var fieldName = nameof(studentAssignment.Grade);
                var errorMessage = "Grade cannot be over 10";
                throw new ArgumentException($"Field: {fieldName}, Error: {errorMessage}");
            }

            if (studentAssignment.Bonus < 0)
            {
                var fieldName = nameof(studentAssignment.Bonus);
                var errorMessage = "Bonus cannot be negative";
                throw new ArgumentException($"Field: {fieldName}, Error: {errorMessage}");
            }
        }

        public Task<List<Student>> GetAllStudents()
        {
            return _repository.GetAllStudents();
        }

        public async Task UpdateStudentAssignmentGrade(Guid assignmentId, decimal newValue)
        {

            await _repository.UpdateStudentAssignmentGrade(assignmentId, newValue);
        }
        public async Task UpdateStudentAssignmentBonus(Guid assignmentId, decimal newValue)
        {

            await _repository.UpdateStudentAssignmentBonus(assignmentId, newValue);
        }

        public Task<List<StudentAssignmentGradingViewModel>> GetStudentAssignmentsForGrading()
        {
            return _repository.GetStudentAssignmentsForGrading();
        }

        public async Task UpdateStudentAssignmentGrade(Guid id, decimal? grade, decimal? bonus)
        {
              await _repository.UpdateStudentAssignmentGrade(id, grade, bonus);
        }

        public async Task RunTests(string filePath, Guid assignmentId, Guid studentId, Guid codeVersionId)
        {
            await _repository.RunTests(filePath, assignmentId, studentId, codeVersionId);
        }
    }
}
