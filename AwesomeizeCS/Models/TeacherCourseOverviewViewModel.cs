using AwesomeizeCS.Domain;

namespace AwesomeizeCS.ViewModels.TeacherOverview;

public class TeacherCourseOverviewViewModel
{
    public Guid SelectedCourseId { get; set; }
    public IEnumerable<Course> Courses { get; set; }
}