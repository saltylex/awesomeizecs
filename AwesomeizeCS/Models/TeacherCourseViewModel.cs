using AwesomeizeCS.Domain;

namespace AwesomeizeCS.Models
{
    public class TeacherCourseViewModel
    {
        public Guid Id { get; set; }
        public Guid TeacherId { get; set; }
        public Course Course { get; set; }
        public bool IsMainTeacher { get; set; }
        public string Username { get; set; }
        
    }
}
