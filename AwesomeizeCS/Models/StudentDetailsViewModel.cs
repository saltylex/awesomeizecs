using AwesomeizeCS.Domain;

namespace AwesomeizeCS.Models;

public class StudentDetailsViewModel
{
    public StudentCourse StudentCourse { get; set; }
    public string CourseName { get; set; }
    public string StudentName { get; set; }
    public List<StudentAssignment> CourseAssignments { get; set; } = new List<StudentAssignment>();
    public List<StudentAssignment> LaboratoryAssignments { get; set; } = new List<StudentAssignment>();
    public List<StudentAssignment> SeminarAssignments { get; set; } = new List<StudentAssignment>();
    public int AssignmentAmount { get; set; }

    public int CourseAttendances { get; set; }
    public int LaboratoryAttendances { get; set; }
    public int SeminarAttendances { get; set; }

    public decimal GradeSoFar { get; set; }
}