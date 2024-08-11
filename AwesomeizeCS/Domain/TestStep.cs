using System.ComponentModel.DataAnnotations.Schema;

namespace AwesomeizeCS.Domain
{
    [Table("TestStep")]
    public class TestStep
    {
        public Guid Id { get; set; }
        public IOTest? IOTest { get; set; }
        public int Order { get; set; } //the step in the test
        public string ProvidedInput { get; set; } //input. ex 1
        public string ExpectedOutput { get; set; } = string.Empty; // nothing or c or success message or stringempty. regex pattern for output
    }
}
