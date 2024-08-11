using TechnologySandbox.Data;

namespace AwesomeizeCS.Domain
{
    public class TimeTable
    {
        public Guid Id { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public InstructionType Type { get; set; }
        public string For { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public int Week { get; set; }
        public int Order { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public Guid TaughtBy { get; set; }
        public AttendanceTracking? SpecificAttendanceTracking { get; set; }

        public Course? Course { get; set; }
    }
}
