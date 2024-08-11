using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;

namespace AwesomeizeCS.Repositories.Interfaces;

public interface IStudentsRepository
{
    ValueTask<Student?> GetStudentById(Guid? id);
    Task<Student?> GetStudentOrDefault(Guid? id);
    Task<List<Student>> GetAllStudents();
    bool StudentExists(Guid id);
    Task CreateStudent(Student Student);
    Task UpdateStudent(Student Student);
    Task DeleteStudent(Student Student);
    Task CreateMissingStudents(List<StudentCourseViewModel> studentList);
}