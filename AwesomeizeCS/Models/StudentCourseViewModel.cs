using AwesomeizeCS.Domain;

namespace AwesomeizeCS.Models
{
    public class StudentCourseViewModel
    {
        public Guid Id { get; set; }
        public string AttendingGroup { get; set; } = string.Empty;
        public string StudentEmail { get; set; }
        public string CourseName { get; set; }
        public string StudentFirstName { get; set; }  
        public string StudentLastName { get; set;}
    }
}
