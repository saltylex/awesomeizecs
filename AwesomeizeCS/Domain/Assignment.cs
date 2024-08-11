using TechnologySandbox.Data;

namespace AwesomeizeCS.Domain
{
    public class Assignment
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public string ShortDescription { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int VisibleFromWeek { get; set; }
        public int SolvableFromWeek { get; set; }
        public int SolvableToWeek { get; set; }
        public InstructionType Type { get; set; }
        public bool HasGrade { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? PercentageOutOfTotal { get; set; }

        public List<IOTest>? Tests { get; set; }
        public List<StudentAssignment>? StudentAssignments { get; set; }
        public Assignment? Parent { get; set; }
        public Course? Course { get; set; }
    }
}