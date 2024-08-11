namespace AwesomeizeCS.Domain
{
    public class Attendance
    {
        public Guid Id { get; set; }
        public bool IsValidated { get; set; }

        public TimeTable Time { get; set; }
        public StudentCourse StudentCourse { get; set; }
    }
}
