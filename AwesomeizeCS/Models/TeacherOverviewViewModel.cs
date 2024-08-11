using AwesomeizeCS.Domain;

namespace AwesomeizeCS.Models;

public class TeacherOverviewViewModel
{
    public Guid Id { get; set; }
    public Attendance? Attendance { get; set; }
    public TimeTable? Time { get; set; }
    public StudentCourse? StudentCourse { get; set; }
    public string? StudentName { get; set; }
    public string? CourseName { get; set; }
    public List<StudentAssignment>? Assignments { get; set; }
}