using System.ComponentModel.DataAnnotations.Schema;

namespace AwesomeizeCS.Domain
{
    [Table("TestResult")]
    public class TestResult
    {
        public Guid Id { get; set; }
        public IOTest Test { get; set; }
        public string Result { get; set; }
        public string Output { get; set; }
    }
}
