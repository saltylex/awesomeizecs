using AwesomeizeCS.Data;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AwesomeizeCS.Repositories;

public class AssignmentsRepository : IAssignmentsRepository
{
    private readonly ApplicationDbContext _db;

    public AssignmentsRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public ValueTask<Assignment?> GetAssignmentById(Guid? id)
    {
        return _db.Assignment.FindAsync(id);
    }

    public Task<Assignment?> GetAssignmentOrDefault(Guid? id)
    {
        return _db.Assignment.Include(a => a.Course).Include(a=>a.Parent).FirstOrDefaultAsync(b => b.Id == id);
    }

    public Task<List<Assignment>> GetAllAssignments()
    {
        return _db.Assignment.Include(sa => sa.Course).OrderBy(a => a.Course.Name).ThenBy(a => a.Name).ToListAsync();
    }

    public Task<List<Assignment>?> GetChildrenAssignments(Assignment assignment)
    {
        return _db.Assignment.Include(a => a.Course).Where(a => a.Parent == assignment).OrderBy(a => a.Course.Name)
            .ThenBy(a => a.Name).ToListAsync();
    }

    public async Task<List<Assignment>> GetAllSubproblems(Guid id)
    {
        var parent = await GetAssignmentById(id);
        return _db.Assignment.Include(a => a.Course).Include(a=>a.Parent).Where(a => a.Parent == parent).ToList();
    }

    public Task<List<Course>> GetAllCourses()
    {
        return _db.Course.ToListAsync();
    }

    public bool AssignmentExists(Guid id)
    {
        return _db.Assignment.Any(a => a.Id == id);
    }

    public async Task CreateAssignment(Assignment assignment)
    {
        assignment.Id = Guid.NewGuid();
        _db.Add(assignment);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAssignment(Assignment assignment)
    {
        _db.Update(assignment);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAssignment(Assignment assignment)
    {
        _db.Assignment.Remove(assignment);
        await _db.SaveChangesAsync();
    }

    public ValueTask<Course?> GetCourseById(Guid courseId)
    {
        return _db.Course.FindAsync(courseId);
    }
}