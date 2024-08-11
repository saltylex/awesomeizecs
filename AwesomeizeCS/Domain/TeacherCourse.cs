namespace AwesomeizeCS.Domain
{
    public class TeacherCourse
    {
        public Guid Id { get; set; }
        public Guid TeacherId { get; set; }
        public Course Course { get; set; }
        public bool IsMainTeacher { get; set; }
    }
}
