using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using AwesomeizeCS.Repositories.Interfaces;
using AwesomeizeCS.Services.Interfaces;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AwesomeizeCS.Services
{
    public class StudentsService : IStudentsService 
    {
        private readonly IStudentsRepository _repository;
        public StudentsService(IStudentsRepository repository)
        {
            _repository = repository;
        }

        public ValueTask<Student?> GetStudentById(Guid? id)
        {
            return _repository.GetStudentById(id);
        }
        public Task<Student?> GetStudentOrDefault(Guid? id)
        {
            return _repository.GetStudentOrDefault(id);
        }

        public Task<List<Student>> GetAllStudents()
        {
            return _repository.GetAllStudents();
        }

        public bool StudentExists(Guid id)
        {
            return _repository.StudentExists(id);
        }

        public async Task CreateStudent(Student student)
        {
            ValidateStudent(student);
            
            await _repository.CreateStudent(student);
        }

        public async Task UpdateStudent(Student student)
        {
            ValidateStudent(student);

            await _repository.UpdateStudent(student);
        }

        public async Task DeleteStudent(Student student)
        {
            await _repository.DeleteStudent(student);
        }

        private void ValidateStudent(Student student)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(student.EmailAddress, pattern))
            {
                var fieldName = nameof(student.EmailAddress);
                var errorMessage = "Email address isn't valid";
                throw new ArgumentException($"Field: {fieldName}, Error: {errorMessage}");
            }
            pattern = @"[0-9]{3}-[12]";
            if (!Regex.IsMatch(student.Subgroup, pattern))
            {
                var fieldName = nameof(student.Subgroup);
                var errorMessage = "subgroup is invalid";
                throw new ArgumentException($"Field: {fieldName}, Error: {errorMessage}");
            }
        }

        public async Task CreateMissingStudents(List<StudentCourseViewModel> studentList)
        {
            await _repository.CreateMissingStudents(studentList);
        }
    }
}
