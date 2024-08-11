using AwesomeizeCS.Domain;

namespace AwesomeizeCS.Models
{
    public class ResultsViewModel
    {
        public Assignment Exercise { get; set; }
        public DateTime RunAt { get; set; }
        public List<TestResultViewModel> TestResults { get; set; }
        public List<string> Errors { get; set; }
    }

    public class TestResultViewModel
    {
        public Guid Id { get; set; }
        public IOTest Test { get; set; }
        public string Result { get; set; }
        public string Output { get; set; }
        public DateTime RunAt { get; set; }
    }
}
