using AwesomeizeCS.Domain;

namespace AwesomeizeCS.Models
{
    public class AttendanceDataViewModel
    {
        public Guid Id { get; set; }
        public bool IsValidated { get; set; }

        public TimeTable Time { get; set; }
        public StudentCourse StudentCourse { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }

    }
}
