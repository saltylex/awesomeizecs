using AwesomeizeCS.Models;

namespace AwesomeizeCS.Models;

    public class StudentOverviewViewModel
    {
        public string CourseName { get; set; }
        public Guid CourseId { get; set; }
        public string AcademicYear { get; set; }

        public List<AssignmentOverviewViewModel> CourseAssignments { get; set; } = new List<AssignmentOverviewViewModel>();
        public List<AssignmentOverviewViewModel> LaboratoryAssignments { get; set; } = new List<AssignmentOverviewViewModel>();
        public List<AssignmentOverviewViewModel> SeminarAssignments { get; set; } = new List<AssignmentOverviewViewModel>();

        public int CourseAttendances { get; set; }
        public int LaboratoryAttendances { get; set; }
        public int SeminarAttendances { get; set; }
        public decimal? GradeSoFar { get; set; }
    }