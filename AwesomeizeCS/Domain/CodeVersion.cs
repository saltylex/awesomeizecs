namespace AwesomeizeCS.Domain
{
    public class CodeVersion
    {
        
        public Guid Id { get; set; }
        public DateTime UploadDate { get; set; }
        public string Location { get; set; }
        public  StudentAssignment CodeFor { get; set; }
        public virtual List<TestResult> Results { get; set; }
    }
}

