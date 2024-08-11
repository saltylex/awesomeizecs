using AwesomeizeCS.Domain;

namespace AwesomeizeCS.Models
{
    public class StudentSituationViewModel
    {
        public Guid Id { get; set; }
        public StudentCourse StudentCourse { get; set; }
        public string StudentName { get; set; }
        public Student Student { get; set; }
        public string CourseName { get; set;}
        public Course Course { get; set;}
        public List<string>? AssignmentsGrades { get; set; }
        public int AttendanceCountLaboratory { get; set; } = 0;
        public int AttendanceCountSeminary { get; set; } = 0;
        public int AttendanceCountCourse { get; set; } = 0;

    }
}
