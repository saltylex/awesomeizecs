using AwesomeizeCS.Domain;

namespace AwesomeizeCS.Models
{
    public class StudentAssignmentGradingViewModel
    {
        public Guid Id { get; set; }
        public decimal? Grade { get; set; }
        public decimal? Bonus { get; set; }

        public Student Student { get; set; }
        public Assignment Assignment { get; set; }

        public string StudentName { get; set; }
        public string AssignmentName {  get; set; }
        public string ShortDescription { get; set; }

        public Course? Course { get; set; }
    }
}
