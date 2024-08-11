using AwesomeizeCS.Data;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using AwesomeizeCS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AwesomeizeCS.Repositories;

public class StudentsRepository : IStudentsRepository
{
    private readonly ApplicationDbContext _db;

    public StudentsRepository (ApplicationDbContext db)
    {
        _db = db;
    }

    public ValueTask<Student?> GetStudentById(Guid? id)
    {
        return _db.Students.FindAsync(id);
    }
    public Task<Student?> GetStudentOrDefault(Guid? id)
    {
        return _db.Students.FirstOrDefaultAsync(b => b.Id == id);
    }

    public Task<List<Student>> GetAllStudents()
    {
        return _db.Students.ToListAsync();
    }

    public bool StudentExists(Guid id)
    {
        return _db.Students.Any(a => a.Id == id);
    }

    public async Task CreateStudent(Student Student)
    {
        Student.Id = Guid.NewGuid();
        _db.Add(Student);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateStudent(Student Student)
    {
        _db.Update(Student);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteStudent(Student Student)
    {
        _db.Students.Remove(Student);
        await _db.SaveChangesAsync();
    }

    public async Task CreateMissingStudents(List<StudentCourseViewModel> studentList)
    {
        foreach(var student in studentList)
        {
            if (_db.Students.FirstOrDefaultAsync(s => s.EmailAddress.Equals(student.StudentEmail)).Result == null)
            {
                var NewStudent = new Student
                {
                    Id = Guid.NewGuid(),
                    EmailAddress = student.StudentEmail,
                    Subgroup = student.AttendingGroup,
                    FirstName = student.StudentFirstName,
                    LastName = student.StudentLastName,


                };
                _db.Students.Add(NewStudent);
            }
            
        }
        await _db.SaveChangesAsync();
    }
}