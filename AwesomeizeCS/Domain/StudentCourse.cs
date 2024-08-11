namespace AwesomeizeCS.Domain
{
    public class StudentCourse
    {
        public Guid Id { get; set; }
        public string AttendingGroup { get; set; } = string.Empty;

        public Student Student { get; set; }
        public Course Course { get; set; }
        public List<Attendance>? Attendances { get; set; }
    }
}
