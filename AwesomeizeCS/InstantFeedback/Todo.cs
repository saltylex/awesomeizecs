namespace AwesomeizeCS.InstantFeedback
{
    public class Todo
    {
        public int StepNumber { get; set; }
        public string Operation { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("Step: {0} Operation: {1} Value: {2} {3}", StepNumber, Operation, Value, Environment.NewLine);
        }
    }
}
