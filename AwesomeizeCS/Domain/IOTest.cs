using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwesomeizeCS.Domain
{
    [Table("IOTest")]
    public class IOTest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public string Hint { get; set; }
        public List<TestStep> Steps { get; set; }

        public override string ToString()
        {
            string stepsAsString = string.Empty;

            if (Steps != null)
            {
                foreach (var step in Steps.OrderBy(s => s.Order))
                {
                    stepsAsString += string.Format("ProvidedInput: {0}{1}{2}{3}", step.ProvidedInput, "; ExpectedOutput: ", step.ExpectedOutput, Environment.NewLine);
                }
            }

            return stepsAsString;
        }
    }
}
