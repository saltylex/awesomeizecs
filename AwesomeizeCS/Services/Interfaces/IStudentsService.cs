using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;

namespace AwesomeizeCS.Services.Interfaces
{
    public interface IStudentsService
    {
        Task CreateStudent(Student student);
        Task DeleteStudent(Student student);
        Task<List<Student>> GetAllStudents();
        ValueTask<Student?> GetStudentById(Guid? id);
        Task<Student?> GetStudentOrDefault(Guid? id);
        bool StudentExists(Guid id);
        Task UpdateStudent(Student student);
        Task CreateMissingStudents(List<StudentCourseViewModel> studentList);
    }
}