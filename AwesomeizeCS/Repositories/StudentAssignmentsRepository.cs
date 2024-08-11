using AwesomeizeCS.Data;
using AwesomeizeCS.Domain;
using AwesomeizeCS.InstantFeedback;
using AwesomeizeCS.Models;
using AwesomeizeCS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AwesomeizeCS.Repositories;

public class StudentAssignmentsRepository : IStudentAssignmentsRepository
{
    private readonly ApplicationDbContext _db;

    public StudentAssignmentsRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<StudentAssignment?> GetStudentAssignmentById(Guid? id)
    {
        return _db.StudentAssignment.Include(sa => sa.Student).Include(sa => sa.Assignment).ThenInclude(a => a.Course)
            .Where(sc => sc.Id == id).FirstOrDefaultAsync();
    }

    public Task<StudentAssignment?> GetStudentAssignmentOrDefault(Guid? id)
    {
        return _db.StudentAssignment.Include(sa => sa.Student).Include(sa => sa.Assignment)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public Task<List<StudentAssignment>> GetAllStudentAssignments()
    {
        return _db.StudentAssignment.Include(sa => sa.Student).Include(sa => sa.Assignment).OrderBy(sa=>sa.Student.LastName).ThenBy(sa=>sa.Assignment.Name).ToListAsync();
    }

    public bool StudentAssignmentExists(Guid id)
    {
        return _db.StudentAssignment.Any(a => a.Id == id);
    }

    public async Task CreateStudentAssignment(StudentAssignment studentAssignment)
    {
        studentAssignment.Id = Guid.NewGuid();
        _db.Add(studentAssignment);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateStudentAssignment(StudentAssignment studentAssignment)
    {
        _db.Update(studentAssignment);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteStudentAssignment(StudentAssignment studentAssignment)
    {
        _db.StudentAssignment.Remove(studentAssignment);
        await _db.SaveChangesAsync();
    }

    public ValueTask<Student?> GetStudentById(Guid id)
    {
        return _db.Students.FindAsync(id);
    }

    public ValueTask<Assignment?> GetAssignmentById(Guid id)
    {
        return _db.Assignment.FindAsync(id);
    }

    public Task<List<Student>> GetAllStudents()
    {
        return _db.Students.ToListAsync();
    }

    public Task<List<Assignment>> GetAllAssignments()
    {
        return _db.Assignment.ToListAsync();
    }

    public async Task UpdateStudentAssignmentGrade(Guid assignmentId, decimal newValue)
    {
        var studentAssignment = await _db.StudentAssignment.FindAsync(assignmentId);

        if (studentAssignment != null)
        {
            studentAssignment.Grade = newValue;
            await _db.SaveChangesAsync();
        }
    }

    public async Task UpdateStudentAssignmentBonus(Guid assignmentId, decimal newValue)
    {
        var studentAssignment = await _db.StudentAssignment.FindAsync(assignmentId);

        if (studentAssignment != null)
        {
            studentAssignment.Bonus = newValue;
            await _db.SaveChangesAsync();
        }
    }

    public async Task<List<StudentAssignmentGradingViewModel>> GetStudentAssignmentsForGrading()
    {
        var studentAssignmentsForGrading = await (from sa in _db.StudentAssignment
                                                  join s in _db.Students on sa.Student.Id equals s.Id
                                                  join a in _db.Assignment on sa.Assignment.Id equals a.Id
                                                  join c in _db.Course on a.Course.Id equals c.Id
                                                  select new StudentAssignmentGradingViewModel
                                                  {
                                                      Id = sa.Id,
                                                      Grade = sa.Grade,
                                                      Bonus = sa.Bonus,
                                                      Student = s,
                                                      Assignment = a,
                                                      StudentName = s.FirstName + " " + s.LastName,
                                                      AssignmentName = a.Name,
                                                      ShortDescription = a.ShortDescription,
                                                      Course = c
                                                  }
                                                  ).ToListAsync();
        return studentAssignmentsForGrading;
    }

    public async Task UpdateStudentAssignmentGrade(Guid id, decimal? grade, decimal? bonus)
    {
        var studentAssignmentRecord = await _db.StudentAssignment.FindAsync(id);
        if(studentAssignmentRecord != null)
        {
            studentAssignmentRecord.Grade = grade;
            studentAssignmentRecord.Bonus = bonus;
            await _db.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentException($"Student assignment record with ID {id} not found.");
        }
    }

    public async Task RunTests(string filePath,Guid assignmentId, Guid studentId, Guid codeVersionId)
    {

        await new TestRunner(_db).RunTests(filePath, assignmentId, studentId, codeVersionId);
    }
}