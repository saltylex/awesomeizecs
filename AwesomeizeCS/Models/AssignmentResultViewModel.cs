using AwesomeizeCS.Domain;
using TechnologySandbox.Data;

namespace AwesomeizeCS.Models
{
    public class AssignmentResultViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public string ShortDescription { get; set; } = string.Empty;
        public int VisibleFromWeek { get; set; }
        public int SolvableFromWeek { get; set; }
        public int SolvableToWeek { get; set; }
    }
}
