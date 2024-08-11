using TechnologySandbox.Data;

namespace AwesomeizeCS.Domain
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid MainTeacherId { get; set; }
        public CourseLanguage Language { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public int NumberOfCourses { get; set; }
        public int NumberOfSeminars { get; set; }
        public int NumberOfLabs { get; set; }
        public AttendanceTracking DefaultCourseAttendanceTracking { get; set; }
        public AttendanceTracking DefaultSeminarAttendanceTracking { get; set; }
        public AttendanceTracking DefaultLaboratoryAttendanceTracking { get; set; }

        public List<TimeTable>? TimeTable { get; set; }
        public List<StudentCourse>? EnrolledStudents { get; set; }
        public List<Assignment>? Assignments { get; set; }
    }
}
