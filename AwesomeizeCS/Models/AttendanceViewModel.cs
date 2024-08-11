using AwesomeizeCS.Domain;

namespace AwesomeizeCS.Models
{
    public class AttendanceViewModel
    {
            public Guid Id { get; set; }
            public bool IsValidated { get; set; }

            public TimeTable Time { get; set; }
            public StudentCourse StudentCourse { get; set; }
        
    }
}
