namespace AwesomeizeCS.Domain
{
    public class Student
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string Subgroup { get; set; } = string.Empty;

        public List<StudentCourse>? AttendingCourses { get; set; }
        public List<StudentAssignment>? Assignments { get; set; }
    }
}
